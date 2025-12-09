// iaService.ts completo (VERSÃO DO SEU PC / HEAD)
import axios from 'axios';

export interface GeminiResponse {
  candidates: {
    content: {
      parts: {
        text: string;
      }[];
    };
    finishReason: string;
  }[];
}

export interface Transaction {
  id: string;
  date: string;
  description: string;
  category: string;
  amount: number;
  type: 'income' | 'expense';
}

export interface ChatMessage {
  role: 'user' | 'assistant';
  content: string;
}

export interface AnalysisLevel {
  id: 'bronze' | 'prata' | 'ouro';
  name: string;
  description: string;
  icon: string;
  maxOutputTokens: number;
  temperature: number;
}

export interface AnalysisConfig {
  level: AnalysisLevel;
  includePatterns: boolean;
  includeRecommendations: boolean;
  includeBudget: boolean;
  includeGoals: boolean;
  includeHealth: boolean;
}

const GEMINI_API_KEY = import.meta.env.VITE_GEMINI_API_KEY;
const GEMINI_API_URL =
  'https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent';

export const ANALYSIS_LEVELS: AnalysisLevel[] = [
  {
    id: 'bronze',
    name: 'Bronze',
    description: 'Análise básica com insights gerais',
    icon: 'fa-solid fa-dice-d6',
    maxOutputTokens: 1024,
    temperature: 0.3
  },
  {
    id: 'prata',
    name: 'Prata',
    description: 'Análise intermediária com recomendações práticas',
    icon: 'fas fa-ring',
    maxOutputTokens: 1536,
    temperature: 0.5
  },
  {
    id: 'ouro',
    name: 'Ouro',
    description: 'Análise completa com planejamento detalhado',
    icon: 'fas fa-crown',
    maxOutputTokens: 2048,
    temperature: 0.7
  }
];

export const geminiService = {
  async getFinancialAdvice(
    transactions: Transaction[],
    config: AnalysisConfig = getDefaultConfig()
  ): Promise<string> {
    try {
      if (!transactions?.length) {
        throw new Error('Nenhuma transação fornecida para análise');
      }

      const formattedTransactions = transactions
        .sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
        .map(
          t =>
            `- ${t.date}: ${t.type === 'expense' ? '-' : '+'}R$${t.amount.toFixed(
              2
            )} (${t.category}) - ${t.description}`
        )
        .join('\n');

      const totalIncome = transactions
        .filter(t => t.type === 'income')
        .reduce((sum, t) => sum + t.amount, 0);

      const totalExpenses = transactions
        .filter(t => t.type === 'expense')
        .reduce((sum, t) => sum + t.amount, 0);

      const balance = totalIncome - totalExpenses;

      const expenseCategories = [
        ...new Set(
          transactions.filter(t => t.type === 'expense').map(t => t.category)
        )
      ];

      const categoryTotals = transactions
        .filter(t => t.type === 'expense')
        .reduce((acc, t) => {
          acc[t.category] = (acc[t.category] || 0) + t.amount;
          return acc;
        }, {} as Record<string, number>);

      const categoryAnalysis = Object.entries(categoryTotals)
        .map(([category, total]) => `- ${category}: R$${total.toFixed(2)}`)
        .join('\n');

      const prompt = this.buildAnalysisPrompt({
        transactions: formattedTransactions,
        summary: {
          totalIncome,
          totalExpenses,
          balance,
          expenseCategories,
          categoryAnalysis
        },
        config
      });

      const response = await this.callGeminiAPI(prompt, config.level);
      return response;
    } catch (error) {
      console.error('Erro na API Gemini:', error);
      return `Desculpe, houve um erro ao gerar sua análise. Detalhes: ${
        error instanceof Error ? error.message : 'Erro desconhecido'
      }`;
    }
  },

  buildAnalysisPrompt(params: {
    transactions: string;
    summary: {
      totalIncome: number;
      totalExpenses: number;
      balance: number;
      expenseCategories: string[];
      categoryAnalysis: string;
    };
    config: AnalysisConfig;
  }): string {
    const { transactions, summary, config } = params;
    const { level } = config;

    const basePrompt = `
      Você é <strong>Penny</strong>, uma assistente financeira IA. 
      Nível de análise: ${level.name.toUpperCase()}
      
      DADOS DO CLIENTE:
      ${transactions}

      RESUMO FINANCEIRO:
      - Renda Total: R$${summary.totalIncome.toFixed(2)}
      - Despesas Totais: R$${summary.totalExpenses.toFixed(2)}
      - Saldo Atual: R$${summary.balance.toFixed(2)}
      - Análise por Categoria:
      ${summary.categoryAnalysis}

      REGRAS GERAIS:
      - Linguagem clara e motivadora
      - Formato markdown
      - Destaque valores importantes em <strong>negrito</strong>
      - Seja específico e acionável
    `;

    const bronzePrompt = `
      ${basePrompt}
      
      FORNECER EM MARKDOWN:
      ### 📊 Resumo Executivo
      - Ganhos totais: R$${summary.totalIncome.toFixed(2)}
      - Despesas totais: R$${summary.totalExpenses.toFixed(2)}
      - Saldo final: R$${summary.balance.toFixed(2)}
      
      ### 🔍 Principais Observações
      - Identifique 1-2 padrões mais evidentes
      - Destaque a categoria com maior gasto
      - Menção breve sobre saúde financeira
      
      ### 💡 Dica Rápida
      - Uma recomendação prática principal
      
      LIMITAÇÕES: Resposta objetiva (máx. 3 parágrafos)
    `;

    const prataPrompt = `
      ${basePrompt}
      
      FORNECER EM MARKDOWN:
      ### 📊 Panorama Financeiro
      - **Ganhos Totais**: R$${summary.totalIncome.toFixed(2)}
      - **Despesas Totais**: R$${summary.totalExpenses.toFixed(2)}
      - **Saldo Final**: R$${summary.balance.toFixed(2)}
      
      ### 🔍 Análise de Padrões
      - Identifique 2-3 padrões de gastos principais
      - Compare categorias com maiores valores
      - Destaque sazonalidades ou hábitos recorrentes
      
      ### 💡 Recomendações Práticas
      - Sugira 2 categorias para otimização
      - Ofereça alternativas concretas
      - Meta de economia mensal sugerida
      
      ### 📈 Saúde Financeira
      - Avalie situação atual
      - Recomendação básica para reserva
      
      FOCO: Equilíbrio entre detalhamento e praticidade
    `;

    const ouroPrompt = `
      ${basePrompt}
      
      FORNECER EM MARKDOWN:
      ### 📊 Diagnóstico Completo
      - **Renda Total**: R$${summary.totalIncome.toFixed(2)}
      - **Despesas Totais**: R$${summary.totalExpenses.toFixed(2)}
      - **Saldo Atual**: R$${summary.balance.toFixed(2)}
      - **Taxa de Poupança**: ${(
        (summary.balance / summary.totalIncome) *
        100
      ).toFixed(1)}%
      
      ### 🔍 Análise Detalhada de Padrões
      - Identifique 3-4 padrões comportamentais
      - Análise sazonal e comparativa
      - Tendências de crescimento/redução
      - Benchmark com médias do setor
      
      ### 💡 Estratégias de Otimização
      - 3-4 categorias para redução com justificativa
      - Plano de ação específico por categoria
      - Potencial de economia mensal/anual
      - Alternativas inteligentes
      
      ### 📊 Orçamento Personalizado
      - Proposta de alocação percentual ideal
      - Metas por categoria baseada em histórico
      - Projeção de crescimento patrimonial
      
      ### 🎯 Metas SMART
      - 2 metas de curto prazo (1-3 meses)
      - 2 metas de médio/longo prazo (6-12 meses)
      - Plano de ação detalhado para cada meta
      
      ### 🛡️ Saúde Financeira Avançada
      - Análise de reserva de emergência
      - Estratégia de formação de patrimônio
      - Recomendações de proteção financeira
      - Indicadores de alerta e sucesso
      
      FOCO: Plano financeiro completo e personalizado
    `;

    const prompts = {
      bronze: bronzePrompt,
      prata: prataPrompt,
      ouro: ouroPrompt
    };

    return prompts[level.id];
  },

  async chatWithAI(params: {
    message: string;
    transactions: Transaction[];
    chatHistory: ChatMessage[];
  }): Promise<string> {
    try {
      const { message, transactions, chatHistory } = params;

      const formattedHistory = chatHistory
        .map(
          msg => `${msg.role === 'user' ? 'Usuário' : 'Penny'}: ${msg.content}`
        )
        .join('\n');

      const recentTransactions = transactions
        .filter(t => {
          const transactionDate = new Date(t.date);
          const thirtyDaysAgo = new Date();
          thirtyDaysAgo.setDate(thirtyDaysAgo.getDate() - 30);
          return transactionDate >= thirtyDaysAgo;
        })
        .sort(
          (a, b) =>
            new Date(b.date).getTime() - new Date(a.date).getTime()
        );

      const formattedTransactions = recentTransactions
        .map(
          t =>
            `- ${t.date}: ${t.type === 'expense' ? '-' : '+'}R$${t.amount.toFixed(
              2
            )} (${t.category}) - ${t.description}`
        )
        .join('\n');

      const totalIncome = recentTransactions
        .filter(t => t.type === 'income')
        .reduce((sum, t) => sum + t.amount, 0);

      const totalExpenses = recentTransactions
        .filter(t => t.type === 'expense')
        .reduce((sum, t) => sum + t.amount, 0);

      const balance = totalIncome - totalExpenses;

      const prompt = `
        Você é <strong>Penny</strong>, uma assistente financeira IA especializada em ajudar pessoas a gerenciarem suas finanças pessoais.

        HISTÓRICO DA CONVERSA:
        ${formattedHistory}

        DADOS FINANCEIROS RECENTES (últimos 30 dias):
        ${formattedTransactions || 'Nenhuma transação recente registrada'}

        RESUMO FINANCEIRO RECENTE:
        - Renda Recente: R$${totalIncome.toFixed(2)}
        - Despesas Recentes: R$${totalExpenses.toFixed(2)}
        - Saldo Recente: R$${balance.toFixed(2)}

        NOVA MENSAGEM DO USUÁRIO:
        "${message}"

        INSTRUÇÕES:
        1. Responda de forma direta e acionável
        2. Use markdown para formatação (negrito, itálico, listas)
        3. Se a pergunta for sobre gastos, consulte os dados antes de responder
        4. Para perguntas sem contexto financeiro, responda como especialista em finanças
        5. Se precisar de mais informações, peça educadamente
        6. Mantenha o tom amigável e encorajador
        7. Destaque valores importantes em <strong>negrito</strong>
        8. Limite a resposta a 3-5 parágrafos

        EXEMPLOS DE RESPOSTA:
        "Baseado nos seus últimos gastos em alimentação (R$XXX), recomendo..."
        "Você teve R$XXX em renda este mês. Para economizar 10%, seria..."
        "Não encontrei transações em [categoria]. Poderia confirmar se..."
      `;

      const response = await this.callGeminiAPI(prompt, ANALYSIS_LEVELS[1]);
      return response;
    } catch (error) {
      console.error('Erro no chat com a IA:', error);
      return 'Desculpe, tive um problema ao processar sua mensagem. Poderia tentar novamente?';
    }
  },

  async callGeminiAPI(prompt: string, level: AnalysisLevel): Promise<string> {
    try {
      const response = await axios.post<GeminiResponse>(
        `${GEMINI_API_URL}?key=${GEMINI_API_KEY}`,
        {
          contents: [
            {
              parts: [{ text: prompt }]
            }
          ],
          generationConfig: {
            temperature: level.temperature,
            topP: 0.9,
            maxOutputTokens: level.maxOutputTokens
          }
        },
        {
          headers: {
            'Content-Type': 'application/json'
          },
          timeout: 30000
        }
      );

      return (
        response.data?.candidates?.[0]?.content?.parts?.[0]?.text ||
        'Não foi possível gerar a resposta. Tente novamente.'
      );
    } catch (error) {
      console.error('Erro na chamada da API Gemini:', error);
      throw error;
    }
  }
};

function getDefaultConfig(): AnalysisConfig {
  return {
    level: ANALYSIS_LEVELS[0], // Bronze
    includePatterns: true,
    includeRecommendations: true,
    includeBudget: false,
    includeGoals: false,
    includeHealth: true
  };
}
