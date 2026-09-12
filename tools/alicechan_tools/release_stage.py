"""Copies the VRoid exports into a staging folder under their release names.

    uv run --project tools alicechan-release-stage 2.0.0 [--out dist]

Produces AliceChan-<version>-VRM1.vrm, AliceChan-<version>-VRM0.vrm and AliceChan-<version>.fbx.
The version must match package.json.
"""

from __future__ import annotations

import argparse
import shutil
from pathlib import Path

from .common import fail, package_version, repo_root

EXPORTS = {
    "AliceChan-VRM1.vrm": "AliceChan-{version}-VRM1.vrm",
    "AliceChan-VRM0.vrm": "AliceChan-{version}-VRM0.vrm",
    "AliceChan.fbx": "AliceChan-{version}.fbx",
}


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__.strip().splitlines()[0])
    parser.add_argument("version")
    parser.add_argument("--out", default="dist", help="staging folder, relative to the repository")
    args = parser.parse_args()

    if args.version != package_version():
        fail(f"version {args.version} does not match package.json ({package_version()})")

    source = repo_root() / "source" / "exports"
    out = repo_root() / args.out
    out.mkdir(parents=True, exist_ok=True)
    for name, target in EXPORTS.items():
        path = source / name
        if not path.is_file():
            fail(f"{path.relative_to(repo_root())} is missing")
        destination: Path = out / target.format(version=args.version)
        shutil.copyfile(path, destination)
        print(destination.relative_to(repo_root()))
