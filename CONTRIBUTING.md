# Contributing

## Tools

- VRoid Studio 2.14.0 or newer, for `source/AliceChan.vroid`.
- Unity 6000.5.10f1 with the OpenUPM registry reachable, for the project under `unity/`.
  Unity 2022.3.22f1 for `ci/unity-2022.3`.
- [uv](https://docs.astral.sh/uv/) for the helper commands under `tools/`.
- The [Unity CLI](https://docs.unity.com/en-us/unity-cli) (`unity`) to run tests and batch
  builds from a terminal.

## Regenerating the model

The `.vroid` file is the only source. Everything else is exported from it.

1. Open `source/AliceChan.vroid` in VRoid Studio.
2. Export VRM 1.0 to `source/exports/AliceChan-VRM1.vrm` and VRM 0.x to
   `source/exports/AliceChan-VRM0.vrm`, both with mesh merging on, no texture atlas, and no
   polygon or bone reduction.
3. Copy `source/exports/AliceChan-VRM1.vrm` over `unity/Packages/com.yuna0x0.alice-chan/Models/AliceChan.vrm`.
4. Regenerate the derived assets, either from the menu *Tools > Alice Chan > Regenerate derived
   assets* in the `unity/` project or from a terminal:

   ```sh
   unity run unity -- -executeMethod AliceChanAuthoring.Regenerate
   ```

   This rewrites `Models/AliceChan.prefab` (a prefab variant of the imported model), the demo
   scene under `Samples~/Demo`, and `source/exports/AliceChan.fbx` (binary FBX, model only). The
   script is `unity/Assets/Editor/AliceChanAuthoring.cs` and is not part of the package.
5. Open the `unity/` project and check the meta, materials, expressions and spring bones on the
   imported model in the inspector.

## Tests

```sh
unity test unity --mode EditMode
unity test ci/unity-2022.3 --mode EditMode
```

The tests are in `unity/Packages/com.yuna0x0.alice-chan/Tests/Editor` and are not shipped in the
release artifacts.

## Checks

```sh
uv run --project tools alicechan-release-check
```

The helper commands live in `tools/` as a uv project; `--project tools` points uv at it from the
repository root.

Validates `package.json`, the changelog section for the current version, and that tracked text
files contain no em-dashes, emoji or the old project name.

## Continuous integration

`.github/workflows/checks.yml` runs the release check on every pull request, and the EditMode
tests on Unity 6000.5 and 2022.3 through GameCI. The Unity jobs need:

- Repository variable `UNITY_CI_ENABLED` set to `true`. Set it to anything else to stop CI from
  logging into the Unity account, for example after a lockout, without touching the secrets.
- Repository secrets `UNITY_EMAIL` and `UNITY_PASSWORD` of a Unity account used only for CI.
  GameCI activates a Personal seat with them (its "personal" method) and returns the seat after
  the job. No licence file or serial is involved; the `.ulf` route is bound to the machine that
  made the file and no longer applies to Personal seats. Two-factor authentication on the
  account does not interfere.
- A password made of letters, digits and symbols other than `$`, the backtick, `\` and `"`.
  The GameCI CLI inlines the value into a shell command, and those characters are rewritten by
  the shell before Unity sees them, which shows up as `Invalid Credential`. Unity locks the
  account for ten minutes after a few failed logins, so the workflows make one activation
  attempt per job and run the two Unity jobs one after the other.

The jobs are skipped, not failed, while the variable or the secrets are absent, so pull requests
from forks stay green.

The package sample lives in `Samples~`, which a common global ignore rule for editor backups
(`*~`) would hide; the repository `.gitignore` negates that rule for the folder.

## Releasing

1. Set the version in `package.json` and add a `## [x.y.z] - date` section to `CHANGELOG.md`.
2. Merge to `main`. To rehearse, run the release workflow from `main` with *Run workflow*: it
   builds the assets and leaves a draft release, which is inspected and then deleted. It can only
   be dispatched from the default branch.
3. Push a signed tag `vx.y.z`. The release workflow builds and attaches the assets, signs the
   UPM tarball when the `release` environment has `UPM_ORG_ID`, `UPM_SERVICE_ACCOUNT_KEY_ID`
   and `UPM_SERVICE_ACCOUNT_KEY_SECRET`, publishes the release, and pushes to itch.io when
   `BUTLER_API_KEY` is set. Releases are immutable once published.
4. The OpenUPM and VPM listings pick the release up from the tag.

## Style

Text is plain: no em-dashes, no emoji, no self-praise. Commit messages describe what changed.
