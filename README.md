# lscyane.BMS

[![Nuget](https://img.shields.io/nuget/vpre/lscyane.BMS.svg?style=for-the-badge)](https://www.nuget.org/packages/lscyane.BMS/)

BMSフォーマットのパーサー（.NET Standard 2.1 ライブラリ）。  
BMS系ファイルの読み込み・書き出しに対応します。

A parser library for the BMS file format targeting .NET Standard 2.1.  
Supports reading and writing BMS files.

## Features / 機能
- Headers: `#TITLE`, `#ARTIST`, `#SUBARTIST`, `#GENRE`, `#COMMENT`, `#PANEL`, `#PREVIEW`, `#PREIMAGE`, `#STAGEFILE`, `#BACKGROUND`, `#RESULTIMAGE`, `#BPM`, `#DLEVEL`, `#GLEVEL`, `#BLEVEL`, `#PLAYLEVEL`, `#RANK`, `#PLAYER`, `#TOTAL`, `#LNOBJ`
- Definitions: `#WAVxx`, `#VOLUMExx`, `#PANxx`, `#BMPxx`, `#AVIxx`, `#BPMxx`, `#STOPxx`
- Notes: general channels, BGM channel `01` sub-channel handling, bar magnification `#mmm02:<rate>`
- Free text passthrough for unknown lines

Limitations / 制限事項:
- `#RANDOM` / `#IF` / `#ENDIF` are not implemented (TODO)

## Target / 対応環境
- `netstandard2.1`
- Note: .NET Framework is not supported by .NET Standard 2.1

## Install / 導入方法
[NuGet](https://www.nuget.org/packages/lscyane.BMS/) or this repository contains a class library `lscyane.BMS`.

Example (CLI):
- `dotnet build`
- `dotnet add <your-app>.csproj reference ./lscyane.BMS/lscyane.BMS.csproj`

## Usage / 使い方

Shift_JIS is used by default. `CodePagesEncodingProvider` is registered internally when loading/saving.

- Read (読み込み):

```csharp
using System.Text;
using lscyane.BMS;

// Shift_JIS is default if enc == null
var bms = Format.Load("score.bms");

// Access headers, notes, definitions
var title = bms.Header.TITLE;
int noteCount = bms.Notes.Count;
```

- Write (書き出し):

```csharp
using System.Text;
using lscyane.BMS;

var bms = new Format();
bms.Header.TITLE = "Example";

// Write all supported headers (pass empty array to disable filtering)
// Default encoding is Shift_JIS when enc == null
bms.Save(
    path: "out.bms",
    supportedHeaders: System.Array.Empty<string>(),
    enc: Encoding.GetEncoding("Shift_JIS")
);
```

Notes / 注意:
- `supportedHeaders` に空配列を渡すとフィルタされず、サポートするヘッダがすべて出力されます。
- 文字コードを明示しない場合、Shift_JIS が使用されます。

## Contributing / 貢献
Issues / PR: 未定（TBD）。現時点ではバグ報告歓迎です。

## License / ライセンス
Apache License 2.0. See `LICENSE`.

Copyright (c) 2025 茶音 麦粉 (@lscyane)
