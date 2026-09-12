"""Checks run before a release and on every pull request.

- package.json has the expected fields and a semantic version.
- CHANGELOG.md has a section for that version.
- Tracked text files contain none of the forbidden strings.
"""

from __future__ import annotations

import re
import subprocess

from .common import PACKAGE_ID, fail, package_dir, package_manifest, repo_root

SEMVER = re.compile(r"^\d+\.\d+\.\d+(-[0-9A-Za-z.-]+)?$")

# Old identity, em-dash, and emoji ranges. Each entry is (label, regex).
FORBIDDEN = [
    ("old name", re.compile(r"edisonlee", re.IGNORECASE)),
    ("em-dash", re.compile("—")),
    ("emoji", re.compile("[\U0001F300-\U0001FAFF☀-➿]")),
]

# The old package id is a fact the changelog and the OpenUPM alias have to state.
OLD_PACKAGE_ID = "com.edisonlee55.alice-chan"

# Files where the old name is expected, or that are not prose.
FORBIDDEN_SKIP = {".mailmap", "uv.lock", "release_check.py"}
FORBIDDEN_SKIP_DIRS = ("Library/", "basis/.work/", "source/")
TEXT_SUFFIXES = {
    ".md", ".json", ".yml", ".yaml", ".toml", ".cs", ".py", ".txt", ".asmdef",
    ".gitignore", ".gitattributes", ".editorconfig", ".python-version",
}


def check_manifest() -> str:
    manifest = package_manifest()
    for field in ("name", "displayName", "version", "unity", "description", "author", "license"):
        if field not in manifest:
            fail(f"package.json is missing {field}")
    if manifest["name"] != PACKAGE_ID:
        fail(f"package.json name is {manifest['name']}, expected {PACKAGE_ID}")
    if not SEMVER.match(manifest["version"]):
        fail(f"package.json version {manifest['version']} is not a semantic version")
    if manifest.get("dependencies"):
        fail("package.json must not declare UPM dependencies (see README, UniVRM is installed separately)")
    return manifest["version"]


def check_changelog(version: str) -> None:
    text = (package_dir() / "CHANGELOG.md").read_text(encoding="utf-8")
    if not re.search(rf"^## \[{re.escape(version)}\]", text, re.MULTILINE):
        fail(f"CHANGELOG.md has no section for {version}")


def tracked_files() -> list[str]:
    output = subprocess.run(
        ["git", "ls-files", "-z"], cwd=repo_root(), capture_output=True, check=True
    ).stdout
    return [name for name in output.decode("utf-8").split("\0") if name]


def check_forbidden() -> None:
    problems = []
    for name in tracked_files():
        path = repo_root() / name
        if path.name in FORBIDDEN_SKIP or name.startswith(FORBIDDEN_SKIP_DIRS):
            continue
        if path.suffix not in TEXT_SUFFIXES and path.name not in TEXT_SUFFIXES:
            continue
        try:
            text = path.read_text(encoding="utf-8")
        except (UnicodeDecodeError, FileNotFoundError):
            continue
        for line_number, line in enumerate(text.splitlines(), start=1):
            line = line.replace(OLD_PACKAGE_ID, "")
            for label, pattern in FORBIDDEN:
                if pattern.search(line):
                    problems.append(f"{name}:{line_number}: {label}")
    if problems:
        print("\n".join(problems))
        fail(f"{len(problems)} forbidden string(s) found")


def main() -> None:
    version = check_manifest()
    check_changelog(version)
    check_forbidden()
    print(f"ok: {PACKAGE_ID} {version}")
