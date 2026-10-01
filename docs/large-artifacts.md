# Original presentation and demonstration

The upload interfaces available for this cleanup rejected the larger original files. Both are stored losslessly as ordered 8 MiB parts under `archive/large-artifacts/`.

After cloning or downloading and extracting the complete repository, run from its root:

```sh
python3 scripts/restore_artifacts.py
```

On Windows, `py scripts/restore_artifacts.py` also works with Python 3.8 or newer. The script uses only Python's standard library. It verifies every part, assembles both files, and checks the original SHA-256 hashes. It leaves an existing matching file untouched and refuses to overwrite a different file.

| Reconstructed file | Original size |
|---|---|
| `demos/dashboard-demo.mp4` | 17,918,800 bytes |
| `docs/slides/research-dashboard-presentation.pptx` | 22,147,891 bytes |

The assembled MP4 and PPTX are byte-identical to the submitted files, including the presentation's embedded media. The reconstruction was tested in a temporary clean directory. The part files themselves cannot be played or opened as slides; assemble them first. No .NET toolchain is needed for this step.
