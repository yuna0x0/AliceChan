"""Paths and small helpers shared by the commands."""

from __future__ import annotations

import json
import subprocess
import sys
from pathlib import Path

PACKAGE_ID = "com.yuna0x0.alice-chan"
UNITY_PROJECT = "unity"


def repo_root() -> Path:
    """The repository root, found from this file so the commands work from any directory."""
    return Path(__file__).resolve().parents[2]


def package_dir() -> Path:
    return repo_root() / UNITY_PROJECT / "Packages" / PACKAGE_ID


def package_manifest() -> dict:
    with (package_dir() / "package.json").open(encoding="utf-8") as handle:
        return json.load(handle)


def package_version() -> str:
    return package_manifest()["version"]


def fail(message: str) -> None:
    print(f"error: {message}", file=sys.stderr)
    raise SystemExit(1)


def run(args: list[str], cwd: Path | None = None) -> None:
    """Run a command, echoing it first, and stop on a non-zero exit."""
    print("+", " ".join(args), flush=True)
    result = subprocess.run(args, cwd=cwd)
    if result.returncode != 0:
        fail(f"command exited with {result.returncode}")
