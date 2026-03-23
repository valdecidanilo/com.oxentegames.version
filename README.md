# Hyper Version ― `com.oxentegames.version`

> Pequeno pacote UPM para **exibir, versionar e acompanhar builds** em tempo de execução, tanto **na WebGL fora do jogo** quanto **dentro do jogo via overlay controlável por código**.

<img width="700" alt="HyperVersion banner" src="https://dummyimage.com/700x120/234a77/ffffff&text=Hyper-Version+%E2%80%94+Show+Build+Info">

---

## ✨ Recursos

| Funcionalidade | Detalhes |
|---|---|
| **Overlay de Versão no jogo** | Cria um `Canvas` overlay em tempo de execução com release, build, data e ambiente. |
| **Controle em runtime** | A visibilidade dentro do jogo pode ser controlada por código via `ShowVersion.OnShowVersion?.Invoke(true/false)`. |
| **Versão na WebGL** | O `index.html` pode ler o `version.json` diretamente e exibir a versão fora do jogo. |
| **version.json em StreamingAssets** | O arquivo é salvo em `Assets/StreamingAssets/version.json`, permitindo leitura pelo Unity e também pelo template WebGL. |
| **Contador de Builds** | `IPreprocessBuildWithReport` incrementa automaticamente o campo `build` a cada build. |
| **Ambientes** | Popup antes do build para definir `environment` como `dev`, `hml` ou `release`. |
| **Janela de Settings** | `Tools ▸ HyperVersion ▸ Settings` com edição dos dados e preview visual. |
| **Botões rápidos** | Inicialização automática de `Resources` e `StreamingAssets`, além de reset do `version.json`. |

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
| **2.** | Ajuste o que deseja exibir no texto da versão, como build, ambiente e data. |
| **3.** | Configure os campos do `version.json`, incluindo `show_version_web` e `show_version_game`. |
| **4.** | Faça o build normalmente. O pacote atualizará automaticamente o `version.json`. |
| **5.** | Na WebGL, o `index.html` poderá mostrar a versão fora do jogo. |
| **6.** | Dentro do jogo, use `ShowVersion.OnShowVersion?.Invoke(true/false)` para mostrar ou esconder o overlay. |

Exemplo de controle em runtime:

```csharp
using HyperVersion.Core;

ShowVersion.OnShowVersion?.Invoke(true);  // mostrar
ShowVersion.OnShowVersion?.Invoke(false); // esconder
```

---

## ⚙️ Estrutura do `version.json`

```json
{
  "release": "1.3.2",
  "build": "7",
  "date": "2026-03-23 14:21:00",
  "environment": "dev",
  "show_version_web": true,
  "show_version_game": false
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
| `show_version_game` | Define se o overlay começa visível dentro do jogo |

### Local do arquivo

```text
Assets/StreamingAssets/version.json
```

Essa mudança é importante porque `StreamingAssets` permite que:
- o Unity leia o arquivo em runtime
- a `index.html` acesse o mesmo JSON no build WebGL

---

## 🎮 Controle dentro do jogo

O overlay da versão agora pode ser exibido ou escondido dinamicamente com evento global.

```csharp
using HyperVersion.Core;

ShowVersion.OnShowVersion?.Invoke(true);   // mostra
ShowVersion.OnShowVersion?.Invoke(false);  // esconde
```

### Comportamento esperado

- `show_version_game` define o estado inicial
- depois disso, você pode controlar a exibição quando quiser por código
- em `release`, normalmente recomenda-se não exibir

Exemplo de uso em algum manager:

```csharp
using HyperVersion.Core;
using UnityEngine;

public class DebugVersionController : MonoBehaviour
{
    private void Start()
    {
        ShowVersion.OnShowVersion?.Invoke(true);
    }

    public void HideVersion()
    {
        ShowVersion.OnShowVersion?.Invoke(false);
    }

    public void ShowVersionAgain()
    {
        ShowVersion.OnShowVersion?.Invoke(true);
    }
}
```

---

## 🌐 Como editar o `index.html` para funcionar com StreamingAssets

Para que a versão apareça fora do jogo na WebGL, você precisa editar o `index.html` do template WebGL.

### Caminho do template

Exemplo:

```text
Assets/WebGLTemplates/HyperVersionTemplate/index.html
```

Ou no template que você já usa no projeto.

### Passo 1: adicionar o container da versão

Coloque isso no `body`, de preferência perto do canvas do jogo:

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

      if (!shouldShow) return;

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
      label.style.display = "block";
    } catch (error) {
      console.warn("[HyperVersion] Falha ao carregar version.json:", error);
    }
  }

  loadHyperVersion();
</script>
```

---

## ✅ Como isso funciona na prática

Quando a build WebGL for gerada, o arquivo:

```text
StreamingAssets/version.json
```

fica disponível junto aos arquivos da build.

Então o fluxo será:

- a página carrega
- o `index.html` faz `fetch("./StreamingAssets/version.json")`
- se `show_version_web` for `true`, a versão aparece no HTML
- o Unity também lê esse mesmo arquivo internamente
- se `show_version_game` for `true`, o texto começa visível no overlay
- depois você pode controlar a visibilidade via:
  ```csharp
  ShowVersion.OnShowVersion?.Invoke(true);
  ShowVersion.OnShowVersion?.Invoke(false);
  ```

---

## 🛠️ API / Extensão

Exemplo de acesso aos dados já carregados:

```csharp
using HyperVersion.Core;

Debug.Log("Controle de exibição da versão:");
ShowVersion.OnShowVersion?.Invoke(true);
```

Se você quiser expor futuramente uma API como `HyperVersionAPI.Current`, ela pode usar o mesmo `VersionData` carregado de `StreamingAssets`.

---

## 🧩 Resumo da mudança principal

### Antes
- `version.json` ficava em:
  ```text
  Assets/Resources/version.json
  ```
- servia bem para o Unity
- não era a melhor abordagem para leitura direta no `index.html`

### Agora
- `version.json` fica em:
  ```text
  Assets/StreamingAssets/version.json
  ```
- pode ser lido:
  - pelo Unity
  - pelo `index.html`
- a versão no jogo pode ser controlada em runtime com:
  ```csharp
  ShowVersion.OnShowVersion?.Invoke(true);
  ShowVersion.OnShowVersion?.Invoke(false);
  ```

---

## 🎯 Roadmap

- ✅ `ScriptableObject` de configuração
- ✅ Preview ao vivo
- ✅ `StreamingAssets/version.json`
- ✅ Integração com template WebGL
- ✅ Controle de visibilidade em runtime
- ⬜ API pública `HyperVersionAPI.Current`
- ⬜ Integração com CI para build automático
- ⬜ Suporte opcional a Addressables

---

## 💬 Suporte

Abra uma *issue* no repositório ou envie email para:

**contato@oxentegames.com.br**

---

© 2025 Oxente Games — Sinta-se livre para dar ⭐ no repositório.
