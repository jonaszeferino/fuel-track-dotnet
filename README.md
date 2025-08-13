# Sistema de Controle de Combustível

Sistema desenvolvido em Blazor Server para controle de veículos e abastecimentos.

## 🚀 Deploy no Railway

### Pré-requisitos
- Conta no [Railway](https://railway.app)
- Projeto conectado ao GitHub/GitLab
- Banco PostgreSQL configurado

### Passos para Deploy

1. **Fork/Clone este repositório**

2. **Conecte ao Railway**
   - Acesse [railway.app](https://railway.app)
   - Faça login com GitHub/GitLab
   - Clique em "New Project"
   - Selecione "Deploy from GitHub repo"

3. **Configure o Projeto**
   - Selecione seu repositório
   - O Railway detectará automaticamente que é um projeto .NET

4. **Configure as Variáveis de Ambiente**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=sua_string_de_conexao_postgresql
   ```

5. **Deploy Automático**
   - O Railway fará o build e deploy automaticamente
   - A aplicação estará disponível em uma URL gerada

### Configuração do Banco de Dados

1. **Crie um banco PostgreSQL no Railway**
   - Adicione um serviço PostgreSQL
   - Copie a string de conexão

2. **Configure a string de conexão**
   - Vá em Variables
   - Adicione: `ConnectionStrings__DefaultConnection`
   - Valor: string de conexão do PostgreSQL

### Estrutura do Projeto

- **Pages/**: Páginas Blazor
- **Controllers/**: APIs REST
- **Data/**: Modelos e Contexto do Entity Framework
- **Services/**: Lógica de negócio

### Tecnologias

- .NET 9.0
- Blazor Server
- Entity Framework Core
- PostgreSQL
- Bootstrap 5

## 🔧 Desenvolvimento Local

```bash
# Restaurar dependências
dotnet restore

# Executar aplicação
dotnet run

# Build
dotnet build

# Testes
dotnet test
```

## 📱 Funcionalidades

- ✅ Cadastro de Veículos
- ✅ Controle de Abastecimentos
- ✅ Relatórios e Estatísticas
- ✅ API REST completa
- ✅ Interface responsiva

## 🌐 Acesso

Após o deploy, a aplicação estará disponível em:
`https://seu-projeto.railway.app`
