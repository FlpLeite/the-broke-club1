# The Broke Club
> **"The Broke Club, o único grupo que você entra pra sair."**

[![Status](https://img.shields.io/badge/status-em%20desenvolvimento-blue)]()
[![GitHub stars](https://img.shields.io/github/stars/FlpLeite/the-broke-club1)](https://github.com/FlpLeite/the-broke-club1/stargazers)

## Sobre o Projeto

O **The Broke Club** nasceu da paixão por tecnologia e da necessidade real de ajudar pessoas a conquistarem o controle das próprias finanças. Nosso propósito é simples e direto: **quebrar o tabu do descontrole financeiro**, mostrando que é possível organizar, economizar e até investir, mesmo começando do zero.

Se você é da turma que sempre acha que "não sobra nada no fim do mês", esse é o seu lugar. Aqui, acreditamos que todo mundo pode dar o primeiro passo para uma vida financeira mais saudável, de forma descomplicada, didática e moderna.

Desenvolver esse projeto é motivo de orgulho e felicidade para todo o time. Cada funcionalidade, cada tela e cada linha de código são feitas com carinho — e muita vontade de fazer a diferença.

---

## Funcionalidades

- **Cadastro e Login de Usuários**
- **Registro de Ganhos e Gastos**
- **Visualização de Transações**
- **Painel de Resumo Financeiro**
- **Design responsivo e Dark Mode**
- **Planejamento de funcionalidades futuras** (metas, relatórios, integração com WhatsApp e mais!)

---

## Tecnologias Utilizadas

### Front-end
- [Vue.js 3](https://vuejs.org/)
- [Tailwind CSS](https://tailwindcss.com/)
- [Pinia](https://pinia.vuejs.org/) (state management)

### Back-end
- [ASP.NET Core Web API (C#)](https://dotnet.microsoft.com/)
- [PostgreSQL](https://www.postgresql.org/) (Banco de dados relacional)

---

## Como Rodar o Projeto

### Pré-requisitos

- [.NET 8+ SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- [PostgreSQL 15+](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/)

### 1. Clone o repositório

```bash
git clone https://github.com/FlpLeite/the-broke-club1.git
cd the-broke-club1
```

### 2. Configurando o Back-end

```bash
cd TheBrokeClubAPI
dotnet restore
```

- Altere a string de conexão com o banco de dados em `appsettings.json` conforme sua configuração local.

- Rode as migrations para criar as tabelas:

```bash
dotnet ef database update
```

- Inicie a API:

```bash
dotnet run
```

A API ficará disponível em `https://localhost:5001` (por padrão).

### 3. Configurando o Front-end

```bash
cd ../the-broke-club-frontend
npm install
npm run dev
```

O front ficará em `http://localhost:5173` (ou porta configurada).

---

## 🛠️ Estrutura do Projeto

```
the-broke-club1/
│
├── TheBrokeClubAPI/          # Back-end C# (API REST)
├── the-broke-club-frontend/  # Front-end VueJS
├── README.md
└── LICENSE
```

---
🚀 Deploy
O projeto ainda está em fase de desenvolvimento, mas em breve estará disponível para acesso online!

🧪 Ambiente de testes: em breve

🌐 Versão pública (produção): em breve

📱 App mobile: planejado para fases futuras

Fique de olho! 🚨 Atualizaremos aqui assim que o deploy estiver disponível.

## 📚 Documentação

- [Rotas da API (em breve)](docs/API.md)
- [Roadmap do Projeto (em breve)](docs/ROADMAP.md)
- [Design System/Themes (em breve)](docs/UI.md)

---

## 💡 Visão Futura

O The Broke Club vai além do básico: queremos integrar **relatórios inteligentes**, **metas financeiras personalizadas**, **alertas de gastos**, **integração com WhatsApp**, dicas de educação financeira, gamificação e comunidade.

**Sua sugestão é sempre bem-vinda!** Sinta-se livre para abrir issues, dar ideias ou até contribuir com código.

---

## 🤝 Contribua

1. Fork este repositório
2. Crie sua branch (`git checkout -b feature/minha-ideia`)
3. Commit suas alterações (`git commit -m 'feat: Minha nova funcionalidade'`)
4. Push na branch (`git push origin feature/minha-ideia`)
5. Abra um Pull Request

---

## 👤 Autor

Feito com orgulho por [Felipe Leite](https://github.com/FlpLeite) e colaboradores.

---

## 🖤 Agradecimentos

A todos que acreditam na mudança, na tecnologia acessível e na força de aprender sobre dinheiro mesmo sem nunca ter ouvido falar de CDB, CDI ou Tesouro Direto!
