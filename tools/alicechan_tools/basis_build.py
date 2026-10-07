"""Builds the Basis bundle from the prepared project.

    uv run --project tools alicechan-basis-build --version 2.0.0 --targets windows,linux,android [--work basis/.work]

Runs the Basis project in batch mode with AliceChanBasisBuild.Build through the unity CLI and
checks that a .BEE file came out. Run alicechan-basis-setup first.
"""

from __future__ import annotations

import argparse
import shutil
from pathlib import Path

from .basis_setup import load_pins
from .common import fail, package_version, repo_root, run

TARGETS = {
    "windows": "StandaloneWindows64",
    "linux": "StandaloneLinux64",
    "android": "Android",
}

OUTPUT_FOLDER = "AliceChan"


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__.strip().splitlines()[0])
    parser.add_argument("--version", default=None, help="defaults to the package.json version")
    parser.add_argument("--targets", default="windows,linux,android",
                        help="comma separated: " + ", ".join(TARGETS))
    parser.add_argument("--work", default="basis/.work")
    parser.add_argument("--log", default=None, help="log file (default: <project>/Logs/basis-build.log)")
    args = parser.parse_args()

    version = args.version or package_version()
    targets = []
    for name in args.targets.split(","):
        name = name.strip().lower()
        if name not in TARGETS:
            fail(f"unknown target {name}; choose from {', '.join(TARGETS)}")
        targets.append(TARGETS[name])

    pins = load_pins()
    project = repo_root() / args.work / "Basis" / pins["basis"]["project"]
    if not (project / "Packages" / "manifest.json").is_file():
        fail(f"{project} is not set up; run alicechan-basis-setup first")

    if shutil.which("unity") is None:
        fail("the unity CLI is not on PATH (https://docs.unity.com/en-us/unity-cli)")

    output = project / "AssetBundles" / OUTPUT_FOLDER
    if output.exists():
        shutil.rmtree(output)

    log = Path(args.log) if args.log else project / "Logs" / "basis-build.log"
    log.parent.mkdir(parents=True, exist_ok=True)
    run([
        "unity", "run", str(project), "--no-banner", "--non-interactive", "--",
        "-executeMethod", "AliceChanBasisBuild.Build",
        "-logFile", str(log),
        "-aliceChanVersion", version,
        "-aliceChanTargets", ",".join(targets),
    ])

    bundles = sorted(output.glob("*.BEE")) if output.is_dir() else []
    if not bundles:
        fail(f"no .BEE file in {output}; see {log}")
    for item in sorted(output.iterdir()):
        print(item)
