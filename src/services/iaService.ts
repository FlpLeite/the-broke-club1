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

const GEMINI_API_KEY = import.meta.env.VITE_GEMINI_API_KEY;
const GEMINI_API_URL = 'https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent';

export const geminiService = {
  async getFinancialAdvice(transactions: Transaction[]): Promise<string> {
    try {
      if (!transactions?.length) {
        throw new Error('Nenhuma transação fornecida para análise');
      }

      const formattedTransactions = transactions
        .sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
        .map(t => `- ${t.date}: ${t.type === 'expense' ? '-' : '+'}R$${t.amount.toFixed(2)} (${t.category}) - ${t.description}`)
        .join('\n');

      const totalIncome = transactions
        .filter(t => t.type === 'income')
        .reduce((sum, t) => sum + t.amount, 0);
      
      const totalExpenses = transactions
        .filter(t => t.type === 'expense')
        .reduce((sum, t) => sum + t.amount, 0);
      
      const balance = totalIncome - totalExpenses;
      const expenseCategories = [...new Set(transactions
        .filter(t => t.type === 'expense')
        .map(t => t.category))];

      const prompt = `
        Você é <strong>Penny</strong>, uma assistente financeira IA. Analise este histórico e forneça:

        DADOS DO CLIENTE:
        ${formattedTransactions}

        RESUMO FINANCEIRO:
        - Renda Total: R$${totalIncome.toFixed(2)}
        - Despesas Totais: R$${totalExpenses.toFixed(2)}
        - Saldo Atual: R$${balance.toFixed(2)}
        - Categorias de Gastos: ${expenseCategories.join(', ')}

        FORNECER EM MARKDOWN:
        ### 🔍 Análise de Padrões
        - Identifique 2-3 padrões de gastos principais
        - Destaque sazonalidades ou hábitos recorrentes
        - Mostrar ganhos totais e despesas no começo da analise

        ### 💡 Recomendações de Economia
        - Sugira 3 categorias para redução
        - Ofereça alternativas concretas

        ### 📊 Sugestão de Orçamento
        - Proponha alocação percentual por categoria
        - Baseado nos gastos históricos

        ### 🎯 Metas Financeiras
        - 2 metas SMART personalizadas
        - Com prazos e valores específicos

        ### 🛡️ Saúde Financeira
        - Avalie situação de emergência
        - Recomende valor para reserva

        REGRAS:
        - Linguagem clara e motivadora
        - Formato markdown com títulos
        - Destaque valores importantes em <strong>negrito</strong> e seu nome
        - Seja específico e acionável
        - Caso o usuario tenha um controle estavel elogie ele por isso explicando pontos impotantes que podem prejudica-lo caso nao tome cuidado, diga isso na saude financeira
        - Caso o usuario nao tenha controle ajude-o com isso e recomende oque fazer
      `;

      const response = await this.callGeminiAPI(prompt);
      return response;
    } catch (error) {
      console.error('Erro na API Gemini:', error);
      return `Desculpe, houve um erro ao gerar sua análise. Detalhes: ${
        error instanceof Error ? error.message : 'Erro desconhecido'
      }`;
    }
  },

  async chatWithAI(params: {
    message: string;
    transactions: Transaction[];
    chatHistory: ChatMessage[];
  }): Promise<string> {
    try {
      const { message, transactions, chatHistory } = params;

      // Formata o histórico de conversa
      const formattedHistory = chatHistory
        .map(msg => `${msg.role === 'user' ? 'Usuário' : 'Penny'}: ${msg.content}`)
        .join('\n');

      // Formata as transações mais recentes (últimos 30 dias por padrão)
      const recentTransactions = transactions
        .filter(t => {
          const transactionDate = new Date(t.date);
          const thirtyDaysAgo = new Date();
          thirtyDaysAgo.setDate(thirtyDaysAgo.getDate() - 30);
          return transactionDate >= thirtyDaysAgo;
        })
        .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());

      const formattedTransactions = recentTransactions
        .map(t => `- ${t.date}: ${t.type === 'expense' ? '-' : '+'}R$${t.amount.toFixed(2)} (${t.category}) - ${t.description}`)
        .join('\n');

      // Cálculos financeiros básicos
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

      const response = await this.callGeminiAPI(prompt);
      return response;
    } catch (error) {
      console.error('Erro no chat com a IA:', error);
      return 'Desculpe, tive um problema ao processar sua mensagem. Poderia tentar novamente?';
    }
  },

   async callGeminiAPI(prompt: string): Promise<string> {
    try {
      const response = await axios.post<GeminiResponse>(
        `${GEMINI_API_URL}?key=${GEMINI_API_KEY}`,
        {
          contents: [{
            parts: [{ text: prompt }]
          }],
          generationConfig: {
            temperature: 0.7,
            topP: 0.9,
            maxOutputTokens: 2048
          }
        },
        {
          headers: {
            'Content-Type': 'application/json'
          },
          timeout: 30000
        }
      );

      return response.data?.candidates?.[0]?.content?.parts?.[0]?.text || 
        'Não foi possível gerar a resposta. Tente novamente.';
    } catch (error) {
      console.error('Erro na chamada da API Gemini:', error);
      throw error;
    }
  }
};