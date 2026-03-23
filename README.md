# Hyper Version ― `com.oxentegames.version`

> Pequeno pacote UPM para **versionar e acompanhar builds** em tempo de execução, com exibição da versão diretamente na **WebGL via `index.html`**, usando `StreamingAssets` e controle opcional a partir do Unity via `.jslib`.

<img width="700" alt="HyperVersion banner" src="https://dummyimage.com/700x120/234a77/ffffff&text=Hyper-Version+%E2%80%94+Show+Build+Info">

---

## ✨ Recursos

| Funcionalidade | Detalhes |
|---|---|
| **version.json em StreamingAssets** | O arquivo é salvo em `Assets/StreamingAssets/version.json`, permitindo leitura pelo Unity e pelo template WebGL. |
| **Versão na WebGL** | O `index.html` pode ler o `version.json` diretamente e exibir a versão fora do jogo. |
| **Controle via Unity** | O Unity pode mostrar ou esconder a versão no HTML chamando funções JavaScript via `.jslib`. |
| **Contador de Builds** | `IPreprocessBuildWithReport` incrementa automaticamente o campo `build` a cada build. |
| **Ambientes** | Popup antes do build para definir `environment` como `dev`, `hml` ou `release`. |
| **Janela de Settings** | `Tools ▸ HyperVersion ▸ Settings` com edição dos dados do `version.json`. |
| **Inicialização automática** | Cria automaticamente `Resources`, `StreamingAssets` e `Plugins/WebGL` com os arquivos necessários. |

---

## 📦 Instalação

1. No **Package Manager**, clique em **+ ▸ Add package from Git URL…**
2. Insira:
   ```text
   https://github.com/oxentegames/HyperVersion.git
   ```
3. Após a importação, abra:
   ```text
   Tools ▸ HyperVersion ▸ Settings
   ```

> **Unity compatível**: 2020.3 LTS+  
> **Licença**: MIT

---

## 🚀 Uso rápido

| Passo | Ação |
|---|---|
| **1.** | Abra **Tools ▸ HyperVersion ▸ Settings**. |
| **2.** | Edite os dados do `version.json`, incluindo `release`, `build`, `environment` e `show_version_web`. |
| **3.** | Faça o build normalmente. O pacote atualizará automaticamente o `version.json`. |
| **4.** | No template WebGL, adicione o script que lê `StreamingAssets/version.json`. |
| **5.** | Se quiser, controle show/hide da versão no HTML chamando Unity → JavaScript com `.jslib`. |

---

## ⚙️ Estrutura do `version.json`

```json
{
  "release": "1.3.2",
  "build": "7",
  "date": "2026-03-23 14:21:00",
  "environment": "dev",
  "show_version_web": true
}
```

### Significado dos campos

| Campo | Descrição |
|---|---|
| `release` | Reflete `PlayerSettings.bundleVersion` |
| `build` | Incrementado automaticamente a cada build |
| `date` | Data/hora da última atualização do arquivo |
| `environment` | Ambiente do build: `dev`, `hml` ou `release` |
| `show_version_web` | Controla se a versão aparece no `index.html` |

### Local do arquivo

```text
Assets/StreamingAssets/version.json
```

---

## 🌐 Exibição da versão no `index.html`

Para que a versão apareça fora do jogo na WebGL, edite o `index.html` do seu template WebGL.

### Caminho do template

Exemplo:

```text
Assets/WebGLTemplates/HyperVersionTemplate/index.html
```

Ou o template que você já usa no projeto.

### Passo 1: adicionar o container da versão

Coloque isso no `body`, próximo ao canvas do jogo:

```html
<div id="version-label" style="display:none; position:absolute; right:12px; bottom:12px; color:white; font-family:Arial, sans-serif; font-size:14px; z-index:9999;">
  --
</div>
```

### Passo 2: adicionar o script que lê o `StreamingAssets/version.json`

Coloque esse script antes do fechamento do `</body>`:

```html
<script>
  async function loadHyperVersion() {
    try {
      const response = await fetch("./StreamingAssets/version.json?ts=" + Date.now(), {
        cache: "no-store"
      });

      if (!response.ok) {
        throw new Error("version.json não encontrado");
      }

      const data = await response.json();

      const shouldShow =
        data.show_version_web === true &&
        data.environment !== "release";

      let version = `v${data.release ?? "0.0.0"}`;

      if (data.build) {
        version += `.${data.build}`;
      }

      if (data.environment && data.environment !== "release") {
        version += `-${data.environment}`;
      }

      const label = document.getElementById("version-label");
      if (!label) return;

      label.textContent = version;
      label.style.display = shouldShow ? "block" : "none";
    } catch (error) {
      console.warn("[HyperVersion] Falha ao carregar version.json:", error);
    }
  }

  window.HyperVersion_Show = function () {
    const label = document.getElementById("version-label");
    if (label) label.style.display = "block";
  };

  window.HyperVersion_Hide = function () {
    const label = document.getElementById("version-label");
    if (label) label.style.display = "none";
  };

  loadHyperVersion();
</script>
```

---

## 🎮 Controle da versão a partir do Unity

O pacote não cria mais overlay visual dentro do Unity. Agora, a exibição é feita no HTML, e o Unity pode controlar isso chamando funções JavaScript.

### Exemplo de uso no Unity

```csharp
using HyperVersion.Core;

HyperVersionWebController.Show();
HyperVersionWebController.Hide();
```

### Como funciona

- `HyperVersionWebController.Show()` chama a função JavaScript `HyperVersion_Show`
- `HyperVersionWebController.Hide()` chama a função JavaScript `HyperVersion_Hide`
- essas funções atuam diretamente no elemento `#version-label` do `index.html`

---

## 🧩 Arquivo `.jslib`

O pacote cria automaticamente o arquivo:

```text
Assets/Plugins/WebGL/HyperVersionWebGL.jslib
```

Conteúdo:

```javascript
mergeInto(LibraryManager.library, {
  HyperVersionShow: function () {
    if (window.HyperVersion_Show) window.HyperVersion_Show();
  },

  HyperVersionHide: function () {
    if (window.HyperVersion_Hide) window.HyperVersion_Hide();
  }
});
```

---

## 🗂️ Estrutura final do pacote

### Mantidos

- `VersionData.cs`
- `HyperVersionWebController.cs`
- `VersionJsonManager.cs`
- `HyperVersionProjectInitializer.cs`
- `HyperVersionSettingsWindow.cs`
- `HyperVersionSettings.cs`
- `BuildTagSelectorWindow.cs`
- `package.json`
- `.asmdef`

### Gerados automaticamente no projeto

- `Assets/StreamingAssets/version.json`
- `Assets/Resources/HyperVersionSettings.asset`
- `Assets/Plugins/WebGL/HyperVersionWebGL.jslib`

### Removidos

- `VersionInitialize.cs`
- `ShowVersion.cs`
- `ResourcesVersionCreator.cs`

---

## ✅ Como isso funciona na prática

Quando a build WebGL for gerada:

- o `version.json` estará disponível em `StreamingAssets/version.json`
- o `index.html` fará `fetch` desse arquivo
- se `show_version_web` for `true` e o ambiente não for `release`, a versão será exibida
- o Unity poderá mostrar ou esconder essa versão chamando `HyperVersionWebController.Show()` e `HyperVersionWebController.Hide()`

---

## 🛠️ API / Extensão

Exemplo simples:

```csharp
using HyperVersion.Core;

public class VersionActionsExample
{
    public void ShowVersion()
    {
        HyperVersionWebController.Show();
    }

    public void HideVersion()
    {
        HyperVersionWebController.Hide();
    }
}
```

---

## 🧠 Observações

- O `version.json` não precisa mais ficar em `Resources`
- A UI da versão não é mais criada dentro do Unity
- Toda a exibição visual da versão ocorre no `index.html`
- O Unity atua apenas como controlador opcional da visibilidade no HTML

---

## 🎯 Roadmap

- ✅ `StreamingAssets/version.json`
- ✅ Integração com template WebGL
- ✅ Controle de show/hide via Unity → JavaScript
- ✅ Build counter automático
- ⬜ API pública para leitura centralizada dos dados
- ⬜ Integração com CI para build automático
- ⬜ Suporte opcional a Addressables

---

## 💬 Suporte

Abra uma *issue* no repositório ou envie email para:

**contato@oxentegames.com.br**

---

© 2025 Oxente Games — Sinta-se livre para dar ⭐ no repositório.
