# api-goncalo&rafael-projeto-final

API REST desenvolvida em **.NET 8 (ASP.NET Core)** para o website **Quinta da Azenha**.

Projeto Final da UC 00605 - Programar para a web, na vertente servidor (server-side).

---

## Colaboradores

| Nome          | GitHub                                             |
| ------------- | -------------------------------------------------- |
| Gonçalo Chora | [@DEVGCh](https://github.com/DEVGCh)              |
| Rafael Lopes  | [@rafaellopes03](https://github.com/rafaellopes03) |

---

## Tecnologias

| Camada | Tecnologia |
|---|---|
| Frontend | HTML5, CSS3, JavaScript, Bootstrap 5 |
| Backend | ASP.NET Core 8 (C#) |
| ORM | Entity Framework Core 8 |
| Base de dados | SQL Server Express |
| Cache distribuído | Redis (StackExchange.Redis) |
| Cache local | Polly (in-memory) |
| Resiliência | Polly (retry + circuit breaker) |
| Mock externo | Mountebank |
| Autenticação | JWT (JSON Web Tokens) |
| Documentação | Swagger / Swashbuckle |
| Containerização | Docker + docker-compose |

---

## Arquitetura do Sistema

```
Website/App
    ↓ HTTPS / JSON
API REST (.NET 8)
    ↓          ↓           ↓           ↓
Polly Cache   Redis Cache  SQL Server  Mountebank
(1º nível)   (2º nível)   (Fallback)  (Mock)
```

O fluxo de cache funciona em cascata:
1. API verifica **Polly** (cache in-memory local)
2. Se não existir → verifica **Redis** (cache distribuído)
3. Se não existir → consulta a **base de dados**
4. Para inventário e pagamentos → comunica com o **Mountebank**

---

## Estrutura do Repositório

```
605_Projeto_API/
└── backend_api/
    ├── visualstudio/
    │   ├── 605_api/
    │   │   ├── Controllers/
    │   │   │   ├── AuthController.cs
    │   │   │   ├── VinhosController.cs
    │   │   │   ├── ExperienciasController.cs
    │   │   │   ├── ReservasController.cs
    │   │   │   ├── InventarioController.cs
    │   │   │   └── UtilizadoresController.cs
    │   │   ├── Models/
    │   │   │   ├── Vinho.cs
    │   │   │   ├── Experiencia.cs
    │   │   │   ├── Reserva.cs
    │   │   │   └── Utilizador.cs
    │   │   ├── Data/
    │   │   │   └── AppDbContext.cs
    │   │   ├── Services/
    │   │   │   └── RedisCacheService.cs
    │   │   ├── Resilience/
    │   │   │   └── ResilienceService.cs
    │   │   ├── DTOs/
    │   │   │   └── LoginDTO.cs
    │   │   ├── Program.cs
    │   │   └── appsettings.json
    │   └── 605_api.sln
    ├── frontend/
    │   ├── css/
    │   │   └── style.css
    │   ├── img/
    │   ├── js/
    │   │   └── script.js
    │   ├── pag/
    │   │   ├── carrinho.html
    │   │   ├── contacto.html
    │   │   ├── experiencias.html
    │   │   └── vinhos.html
    │   └── index.html
    ├── Imposter/
    │   └── mountebank.json
    └── docker-compose.yml
```

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou VS Code

---

## Como Executar

### 1. Clonar o repositório

```bash
git clone https://github.com/username/api-goncalo&rafael-projeto-final.git
cd api-goncalo&rafael-projeto-final
```

### 2. Iniciar Redis e Mountebank via Docker

Na pasta raiz do projeto (onde está o `docker-compose.yml`):

```bash
docker-compose up -d
```

Verifica que estão a correr:
- Mountebank: http://localhost:2525
- Redis: porta 6379

### 3. Configurar a base de dados

Abre o `appsettings.json` e confirma a connection string:

```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=QuintaAzenhaDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

A base de dados é criada automaticamente ao correr a API pela primeira vez.

### 4. Correr a API

Abre o projeto no Visual Studio 2022 e clica em **Run** (F5), ou:

```bash
cd backend_api/visualstudio/605_api
dotnet run
```

A API fica disponível em:
- **Swagger UI**: https://localhost:7095
- **API Base**: https://localhost:7095/api

### 5. Abrir o Frontend

Abre o ficheiro `frontend/index.html` com o **Live Server** no VS Code.

---

## Endpoints da API

### Auth
| Método | Endpoint | Descrição | Auth |
|---|---|---|---|
| POST | `/api/Auth/register` | Registar utilizador | ❌ |
| POST | `/api/Auth/login` | Login e obtenção de JWT | ❌ |

### Vinhos
| Método | Endpoint | Descrição | Auth |
|---|---|---|---|
| GET | `/api/Vinhos` | Lista todos os vinhos | ❌ |
| GET | `/api/Vinhos/{id}` | Detalhe de um vinho | ❌ |
| POST | `/api/Vinhos` | Criar vinho | ✅ |
| PUT | `/api/Vinhos/{id}` | Editar vinho | ✅ |
| DELETE | `/api/Vinhos/{id}` | Eliminar vinho | ✅ |

### Experiências
| Método | Endpoint | Descrição | Auth |
|---|---|---|---|
| GET | `/api/Experiencias` | Lista todas as experiências | ❌ |
| GET | `/api/Experiencias/{id}` | Detalhe de uma experiência | ❌ |
| POST | `/api/Experiencias` | Criar experiência | ✅ |
| PUT | `/api/Experiencias/{id}` | Editar experiência | ✅ |
| DELETE | `/api/Experiencias/{id}` | Eliminar experiência | ✅ |

### Reservas
| Método | Endpoint | Descrição | Auth |
|---|---|---|---|
| POST | `/api/Reservas` | Criar reserva | ❌ |
| GET | `/api/Reservas` | Lista todas as reservas | ✅ |
| GET | `/api/Reservas/{id}` | Detalhe de uma reserva | ✅ |
| PUT | `/api/Reservas/{id}/estado` | Atualizar estado da reserva | ✅ |
| DELETE | `/api/Reservas/{id}` | Eliminar reserva | ✅ |

### Inventário (Mountebank)
| Método | Endpoint | Descrição | Auth |
|---|---|---|---|
| GET | `/api/Inventario/{sku}` | Verificar stock de um vinho | ✅ |
| POST | `/api/Inventario/pagamento` | Processar pagamento | ✅ |

### Utilizadores
| Método | Endpoint | Descrição | Auth |
|---|---|---|---|
| GET | `/api/Utilizadores` | Lista todos os utilizadores | ✅ |
| GET | `/api/Utilizadores/{id}` | Detalhe de um utilizador | ✅ |
| PUT | `/api/Utilizadores/{id}` | Editar utilizador | ✅ |
| DELETE | `/api/Utilizadores/{id}` | Eliminar utilizador | ✅ |

> ✅ Requer token JWT no header: `Authorization: Bearer {token}`

---

## Autenticação JWT

Para aceder aos endpoints protegidos:

1. Faz `POST /api/Auth/login` com email e password
2. Copia o token da resposta
3. No Swagger clica em **Authorize** e introduz `Bearer {token}`

---

## Cache — Estratégia em Cascata

```
Pedido GET /api/Vinhos
        ↓
  Polly Cache (2 min)  →  HIT: devolve dados
        ↓ MISS
  Redis Cache (10 min) →  HIT: atualiza Polly + devolve dados
        ↓ MISS
  SQL Server           →  atualiza Redis + Polly + devolve dados
```

---

## Resiliência com Polly

- **Retry**: 3 tentativas com 2 segundos de espera entre cada uma
- **Circuit Breaker**: abre após 3 falhas consecutivas, mantém-se aberto 30 segundos
- Aplicado nas chamadas ao Mountebank (inventário e pagamentos)

---

## Mock Externo — Mountebank

Simula dois serviços externos:

**Inventário** - `GET http://localhost:3000/inventory/{sku}`
```json
{ "sku": "QA-001", "nome": "Arinto Clássico", "stock": 45, "disponivel": true }
```

**Pagamentos** - `POST http://localhost:3000/payments`
```json
{ "success": true, "transactionId": "TXN-12345", "mensagem": "Pagamento processado com sucesso" }
```

---

## Base de Dados

A base de dados `QuintaAzenhaDB` é criada automaticamente pelo Entity Framework Core ao iniciar a API.

**Tabelas:**
- `Vinhos` — catálogo de vinhos com SKU para inventário
- `Experiencias` — experiências disponíveis na quinta
- `Reservas` — pedidos de reserva do formulário de contacto
- `Utilizadores` — utilizadores com autenticação JWT

**Seed data** incluído automaticamente: 3 vinhos e 4 experiências.
