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

const GEMINI_API_KEY = import.meta.env.VITE_GEMINI_API_KEY;
const GEMINI_API_URL = 'https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent';

export const geminiService = {
  async getFinancialAdvice(transactions: Transaction[]): Promise<string> {
    try {
      if (!transactions?.length) {
        throw new Error('Nenhuma transação fornecida para análise');
      }

      // Formatação melhorada das transações
      const formattedTransactions = transactions
        .sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
        .map(t => `- ${t.date}: ${t.type === 'expense' ? '-' : '+'}R$${t.amount.toFixed(2)} (${t.category}) - ${t.description}`)
        .join('\n');

      // Cálculos financeiros básicos
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
        Você é Penny, uma assistente financeira IA. Analise este histórico e forneça:

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
        - Destaque valores importantes em **negrito**
        - Seja específico e acionável
      `;

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
        'Não foi possível gerar a análise. Tente novamente.';
    } catch (error) {
      console.error('Erro na API Gemini:', error);
      return `Desculpe, houve um erro ao gerar sua análise. Detalhes: ${
        error instanceof Error ? error.message : 'Erro desconhecido'
      }`;
    }
  }
};