# Fork — suporte à pasta `mods` do OpenIV

Fork de [flobros/CodeWalker.API](https://github.com/flobros/CodeWalker.API), mantido para uso
no [GTAVUltimateInstaller](https://github.com/ShadowSsk/GTAVUltimateInstaller).

> Este arquivo documenta **apenas o que muda** em relação ao upstream. O README original
> continua valendo para todo o resto.

## Por que o fork existe

A API oficial **não enxerga a pasta `mods`** do OpenIV. Ela cria o `RpfManager` e chama
`Init()` sem nunca ligar `EnableMods`, e a busca varre só o `EntryDict` — que contém apenas
as entradas do jogo base.

Isso inviabiliza qualquer ferramenta que trabalhe sobre uma instalação modificada. No
GTAVUltimateInstaller, sem isso:

- o backup lê os arquivos originais do jogo em vez das versões já modificadas;
- o passo que busca o `global.gxt2` falha sempre, porque procura em caminhos `mods\...`.

O suporte já existe pronto na `CodeWalker.Core` (`RpfManager.EnableMods`, `ModRpfDict`,
`ModEntryDict`) — a API só não o utilizava.

## O que foi alterado

### `Models/ApiConfig.cs`

Três chaves novas:

| Chave | Padrão | O que faz |
|---|---|---|
| `UseModsFolder` | herda `EnableMods` | Varre a pasta `mods` e expõe o conteúdo dela |
| `PreferModsOverBase` | `true` | Lista primeiro a cópia em `mods` quando o arquivo existe nos dois lugares |
| `ScanDlcPacks` | `true` | `false` pula `update\x64\dlcpacks` — sobe muito mais rápido, mas não acha conteúdo de DLC |

### `Services/RpfService.cs`

- `_rpfManager.EnableMods` passa a ser definido **antes** do `Init()`. Sem isso o manager até
  indexa a pasta `mods` no `ModEntryDict`, mas `GetEntry`/`FindRpfFile` nunca olham para lá —
  e os downloads sempre resolvem para o jogo base.
- `SearchFile` passa a varrer também o `ModEntryDict`, devolvendo caminhos com o prefixo
  `mods\`. O `ModEntryDict` indexa cada entrada **duas vezes** (com e sem o prefixo), então só
  a forma prefixada é mantida, para não duplicar resultados.
- `ScanDlcPacks=false` alimenta o `ExcludePaths` do manager.

### `CodeWalker.API.csproj`

O `ProjectReference` aponta para `..\CodeWalker\CodeWalker.Core\CodeWalker.Core.csproj`.
O upstream espera `..\CodeWalker.Core\`, que não existe em nenhum repositório — o fonte da
Core vem de [dexyfex/CodeWalker](https://github.com/dexyfex/CodeWalker).

## Como compilar

Requer o **.NET 9 SDK**. O fonte da CodeWalker precisa estar ao lado deste repositório:

```
C:\Projetos\
  ├── CodeWalker.API\     (este repositório)
  └── CodeWalker\         (clone de dexyfex/CodeWalker, sem modificações)
```

```bash
git clone https://github.com/dexyfex/CodeWalker.git C:\Projetos\CodeWalker
```

```bash
dotnet publish CodeWalker.API.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -o publish
```

> ⚠️ **Não publique com `PublishSingleFile=true`.** A `CodeWalker.Core` monta o caminho do
> `strings.txt` a partir de `Assembly.GetExecutingAssembly().Location`, que é string vazia em
> single-file. O resultado é `ArgumentNullException` em `BuildBaseJenkIndex` e a API morre ao
> subir, logo depois de terminar o scan.

## Configuração

`Config/userconfig.json`, ao lado do executável:

```json
{
  "GTAPath": "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Grand Theft Auto V",
  "CodewalkerOutputDir": "C:\\GTA_FILES\\cw_out",
  "UseModsFolder": true,
  "PreferModsOverBase": true,
  "ScanDlcPacks": true,
  "Port": 5555
}
```

Para o `UseModsFolder` servir para alguma coisa, a pasta `mods` precisa existir dentro da
instalação do GTA V e conter cópias dos `.rpf` que se quer modificar — é a convenção do OpenIV.

## Verificando que funcionou

Com a pasta `mods` populada, a busca deve devolver caminhos com o prefixo:

```bash
curl "http://localhost:5555/api/search-file?filename=carcols.ymt"
```

```json
["mods\\x64a.rpf\\data\\carcols.ymt", "x64a.rpf\\data\\carcols.ymt", ...]
```

Sem a modificação, **nenhum** resultado vem com `mods\`.

## Licença

MIT, como o upstream. O `LICENSE` original foi preservado.
