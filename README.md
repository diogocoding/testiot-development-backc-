# 🔐 AccessControlAPI — Backend do Sistema de Controle de Acesso IoT

> **Projeto Integrador — CST em Análise e Desenvolvimento de Sistemas — 4º Período**
> Faculdade Senac PE | Componente norteador: IoT (Internet das Coisas)

---

## 👥 Equipe

| Nome | Turma |
|---|---|
| Vinícius Oliveira | ADS 4º Período |
| Pedro Juan | ADS 4º Período |
| Gustavo Henrique | ADS 4º Período |
| Miguel Veloso | ADS 4º Período |
| Carlos Machado | ADS 4º Período |
| Maíra Lourenço | ADS 4º Período |
| Wslany Lima | ADS 4º Período |
| Diogo Nascimento | ADS 4º Período |

---

## 📖 Sobre o Projeto

O **AccessControlAPI** é o backend de uma solução IoT de controle de acesso por RFID para a Faculdade Senac PE. O sistema resolve um problema real: **o controle de entrada e saída em ambientes restritos da faculdade**, permitindo identificar usuários por cartão RFID, autorizar ou bloquear o acesso em tempo real e registrar todo o histórico de entradas.

A API é construída em **ASP.NET Core (.NET 8)**, conectada a um banco de dados PostgreSQL via **Supabase**, implantada na nuvem via **Railway** com Docker, e se comunica diretamente com placas **ESP32** via requisições HTTP/REST.

---

## 🏗️ Arquitetura da Solução

```
[ Cartão RFID ]
      │  leitura
      ▼
[ ESP32 (Firmware C++) ]
      │  POST /api/access/validate  (HTTP/REST)
      │  POST /api/aparelhos/ping   (Heartbeat)
      ▼
[ AccessControlAPI — ASP.NET Core 8 ]  ◄── Railway (Docker)
      │
      ├── AccessController    → valida acesso, registra logs
      ├── UsersController     → CRUD de usuários
      ├── AparelhosController → CRUD de dispositivos + heartbeat
      └── DeviceMonitorService → background worker (status online/offline)
      │
      ▼
[ Supabase — PostgreSQL ]
   ├── Tabela: Usuarios
   ├── Tabela: Aparelhos
   └── Tabela: Logs
      │
      ▼
[ Frontend React / Dashboard ]  ◄── Vercel
```

**Camadas:** Percepção (ESP32 + RFID) → Rede (HTTP REST) → Nuvem/Aplicação (Railway + Supabase + Vercel)

---

## ⚙️ Requisitos Técnicos

- [x] **Sensores:** Leitor RFID (RC522) + sensor de presença (ESP32)
- [x] **Conectividade:** ESP32 com Wi-Fi — comunicação HTTP/REST
- [x] **Mobile/Web:** Dashboard responsivo consumindo esta API
- [x] **Dashboard:** Visualização de logs, status dos dispositivos e gestão de usuários
- [x] **Aplicação no ar:** Deploy em Railway com Docker

---

## 🚀 Endpoints da API

### `POST /api/access/validate`
Recebido pelo ESP32 a cada leitura de cartão. Valida o dispositivo, o usuário e o status de acesso, e registra o log automaticamente.

**Request body:**
```json
{
  "rfidTag": "04:A3:22:1B",
  "deviceToken": "ESP32-ENTRADA-001"
}
```

**Response (sucesso):**
```json
{
  "authorized": true,
  "userName": "Vinícius Oliveira",
  "message": "Acesso Liberado"
}
```

**Response (negado):**
```json
{
  "authorized": false,
  "message": "Acesso bloqueado pelo administrador."
}
```

---

### `GET /api/access/logs`
Retorna o histórico completo de tentativas de acesso, ordenado do mais recente para o mais antigo.

---

### `GET /api/users`
Lista todos os usuários cadastrados.

### `POST /api/users`
Cadastra um novo usuário com nome, cargo e UID do cartão RFID.

### `PUT /api/users/{id}`
Atualiza dados do usuário ou alterna o status ativo/bloqueado (toggle de acesso).

### `DELETE /api/users/{id}`
Remove um usuário do sistema.

---

### `GET /api/aparelhos`
Lista todos os dispositivos ESP32 cadastrados com status online/offline.

### `POST /api/aparelhos`
Cadastra um novo dispositivo.

### `PUT /api/aparelhos/{id}`
Atualiza nome, localização ou token do dispositivo.

### `DELETE /api/aparelhos/{id}`
Remove um dispositivo.

### `POST /api/aparelhos/ping`
**Heartbeat:** recebido periodicamente pelo ESP32 para manter o status "online" atualizado.

```json
{ "deviceToken": "ESP32-ENTRADA-001" }
```

---

## 🗄️ Modelos de Dados

### `Usuario`
| Campo | Tipo | Descrição |
|---|---|---|
| Id | int | Chave primária |
| Name | string | Nome completo |
| Role | string | Cargo (ex: Aluno, Professor) |
| Uid | string | UID do cartão RFID |
| Active | bool | Define se o acesso está liberado |

### `Aparelho`
| Campo | Tipo | Descrição |
|---|---|---|
| Id | int | Chave primária |
| Token | string | Identificador único da placa (ex: ESP32-001) |
| Name | string | Nome descritivo (ex: Entrada Principal) |
| Location | string | Localização física (ex: Térreo - Portão A) |
| IsOnline | bool | Status atual de conectividade |
| LastPing | DateTime? | Timestamp do último heartbeat recebido |

### `LogAcesso`
| Campo | Tipo | Descrição |
|---|---|---|
| Id | int | Chave primária |
| Timestamp | DateTime | Data e hora da tentativa |
| RfidTag | string | UID do cartão lido |
| UserName | string? | Nome do usuário (nulo se cartão desconhecido) |
| DeviceToken | string | Qual ESP32 fez a leitura |
| Authorized | bool | Se o acesso foi liberado ou negado |
| Message | string | Motivo da decisão |

---

## 🔧 Como Executar Localmente

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Docker](https://www.docker.com/) (opcional, para rodar containerizado)
- Acesso a um banco PostgreSQL (Supabase ou local)

### 1. Configurar as credenciais

Crie um arquivo `appsettings.Development.json` baseado no exemplo:

```bash
cp appsettings.example.json appsettings.Development.json
```

Edite com sua connection string real:

```json
{
  "ConnectionStrings": {
    "Supabase": "Host=SEU_HOST;Port=5432;Database=postgres;Username=SEU_USER;Password=SUA_SENHA;"
  }
}
```

> ⚠️ **Nunca commite o arquivo com credenciais reais.** O `appsettings.Development.json` já está no `.gitignore`.

### 2. Rodar a aplicação

```bash
dotnet restore
dotnet run
```

A API estará disponível em `http://localhost:5000`.  
A documentação interativa (Swagger) estará em `http://localhost:5000/swagger`.

### 3. Rodar com Docker

```bash
docker build -t access-control-api .
docker run -p 5000:80 -e ConnectionStrings__Supabase="SUA_CONNECTION_STRING" access-control-api
```

---

## 🔒 Segurança e LGPD

- **Credenciais protegidas:** a connection string nunca é exposta no código. Em produção, é injetada como variável de ambiente no Railway. Localmente, lida via `appsettings.Development.json` (ignorado pelo `.gitignore`).
- **Arquivo de exemplo:** `appsettings.example.json` contém chaves fictícias para orientar novos desenvolvedores sem expor dados reais.
- **Minimização de dados:** o sistema coleta apenas os dados estritamente necessários (UID do cartão, nome, cargo e timestamp), em conformidade com o princípio de minimização da LGPD.
- **Fail-safe:** a lógica de validação sempre nega o acesso por padrão em caso de dispositivo ou cartão não reconhecido, jamais concede acesso em caso de dúvida.
- **CORS configurado:** apenas origens autorizadas (frontend em produção) devem ser liberadas; o modo `AllowAnyOrigin` é adequado para o MVP, mas deve ser restrito em produção.
- **Logs de auditoria:** toda tentativa de acesso — bem-sucedida ou negada — é registrada com timestamp, identificação do dispositivo e motivo da decisão.

---

## 🗂️ Mapeamento de UCs

| Conceito aplicado | UC | Onde está evidenciado |
|---|---|---|
| API REST com ASP.NET Core, endpoints HTTP, payload JSON | Engenharia de Software & APIs | `Controllers/`, `Program.cs` |
| Banco de dados relacional PostgreSQL, Entity Framework Core, migrations | Banco de Dados / Cloud Computing | `Models/AppDbContext.cs`, `Models/*.cs`, Supabase |
| Deploy em nuvem com Docker e Railway, variáveis de ambiente | Cloud Computing | `Dockerfile`, `Program.cs` (PORT dinâmica) |
| Proteção de credenciais, fail-safe, LGPD, minimização de dados | Segurança da Informação | `appsettings.example.json`, `.gitignore`, `AccessController.cs` |
| Background Service para monitoramento de dispositivos IoT | IoT & Artefato | `Services/DeviceMonitorService.cs` |
| Comunicação ESP32 → API via HTTP (protocolo REST) | IoT & Artefato / Eng. de Software | `AccessController.cs`, `AparelhosController.cs` |
| Persona do administrador e do usuário final, jornada de acesso | Comportamento do Consumidor | Diagrama de jornada (ver `/docs`) |
| README e comentários técnicos em inglês | Inglês | Campos dos modelos (`Name`, `Role`, `Authorized`, etc.), este README |

---

## 🧑‍💻 Uso de IA Generativa

A equipe utilizou ferramentas de IA generativa (Claude) como apoio ao desenvolvimento: para sugestões de estrutura de código, revisão de lógica e redação de documentação. Todo o código foi compreendido, revisado e validado pelos membros da equipe, que são capazes de explicar qualquer trecho durante a defesa.

---

## 📁 Estrutura do Repositório

```
AccessControlAPI/
├── Controllers/
│   ├── AccessController.cs       # Validação de acesso e logs
│   ├── AparelhosController.cs    # CRUD de dispositivos + heartbeat
│   └── UsersController.cs        # CRUD de usuários
├── Models/
│   ├── AppDbContext.cs            # Contexto do banco de dados
│   ├── Usuario.cs                 # Modelo de usuário
│   ├── Aparelhos.cs               # Modelo de dispositivo ESP32
│   ├── LogAcesso.cs               # Modelo de log de acesso
│   └── AccessRequest.cs           # DTO da requisição do ESP32
├── Services/
│   └── DeviceMonitorService.cs    # Background worker de heartbeat
├── appsettings.json               # Configurações gerais
├── appsettings.example.json       # Exemplo de configuração (sem credenciais)
├── Dockerfile                     # Build e deploy containerizado
├── AccessControlAPI.csproj        # Definição do projeto .NET
└── README.md
```

---

## 📜 Licença

Projeto desenvolvido para fins acadêmicos — Faculdade Senac PE, 2026.
