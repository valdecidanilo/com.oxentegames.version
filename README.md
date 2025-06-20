# Hyper Version ― `com.oxentegames.version`
> Pequeno pacote UPM para **exibir, versionar e acompanhar builds** em tempo‑de‑execução.

<img width="700" alt="HyperVersion banner" src="https://dummyimage.com/700x120/234a77/ffffff&text=Hyper‑Version+%E2%80%94+Show+Build+Info">

---

## ✨ Recursos

| Funcionalidade | Detalhes |
|---------------|-----------|
| **Overlay de Versão** | Cria um *Canvas* overlay no `BeforeSceneLoad` contendo release, build, data e ambiente. |
| **version.json**      | Arquivo em `Assets/Resources/version.json` atualizado automaticamente no Editor. |
| **Contador de Builds**| `IPreprocessBuild` incrementa o campo **build** a cada Player Build. |
| **Ambientes**         | Popup (Dev / Stg / Release) antes do build ⇢ grava `environment`. |
| **Janela de Settings**| `Tools ▸ HyperVersion ▸ Settings` (pré‑visualização ao vivo). |
| **Botões rápidos**    | *Resetar version.json* e *Inicializar Resources* diretamente da UI. |

---

## 📦 Instalação

1. No **Package Manager** clique **+ ▸ Add package from Git URL…**  
2. Insira:  
   ```
   https://github.com/oxentegames/HyperVersion.git
   ```
3. A janela de **Settings** abrirá na primeira importação.

> **Unity compatível** : 2020.3 LTS + • **Licença** : MIT

---

## 🚀 Uso rápido

| Passo | Ação |
|-------|------|
| **1.** | Abra **Tools ▸ HyperVersion ▸ Settings**. |
| **2.** | Ajuste o que deseja exibir (build, ambiente, data) e veja o *preview*. |
| **3.** | *(Opcional)* clique **Resetar version.json** para reiniciar contadores. |
| **4.** | Construa seu jogo normalmente; no primeiro frame o overlay aparecerá: <br>`v1.3.2-dev/2025-06-20 12:34:56` |

---

## ⚙️ Estrutura do `version.json`

```jsonc
{
  "release"    : "1.3.2",           // refletido de PlayerSettings.bundleVersion
  "build"      : "7",               // incrementado a cada build
  "data"       : "2025-06-20 14:21",
  "environment": "dev"              // dev / stg / release
}
```

*Local*: **Assets/Resources/version.json** (incluído em Resources → seguro para WebGL).

---

## 🛠️ API / Extensão

```csharp
using HyperVersion.Core;

var data = HyperVersionAPI.Current;          // lê o JSON já desserializado
Debug.Log($"Versão atual: {data.release}.{data.build}");
```

---

## 🎯 Roadmap

- ✅ ScriptableObject de configuração  
- ✅ Preview ao vivo  
- ⬜ Suporte a `Addressables`  
- ⬜ Integração com CI para `build` automático  

---

### 💬 Suporte

Abra uma *issue* ou envie email para **contato@oxentegames.com.br**

---

© 2025 Oxente Games – Sinta‑se livre para dar ⭐ no repo!
