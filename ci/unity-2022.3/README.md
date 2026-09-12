# Unity 2022.3 check project

A minimal project on the package's minimum Unity version. It references the package by relative
path and pulls UniVRM from OpenUPM, so the package's EditMode tests run against 2022.3 in CI and
locally:

```sh
unity test ci/unity-2022.3 --mode EditMode
```

Only `ProjectSettings/ProjectVersion.txt` and `Packages/manifest.json` are committed; Unity
generates the rest on first open.
