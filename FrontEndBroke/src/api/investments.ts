import axios from 'axios';

const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL ?? 'http://localhost:5024/api',
    withCredentials: false,
});

export type InvestCard = {
    ativoId: number;
    nome: string;
    ticker: string;
    quantidade: number;
    precoMedio: number;
    valorAtual: number;
    totalInvestido: number;
    ganhoNaoRealizado: number;
    ganhoRealizado: number;
    percentual: number;
    isQuoteStale: boolean;
};

export async function fetchInvestmentCards(usuarioId: number) {
    const { data } = await api.get<InvestCard[]>(`/investimentos/cards`, {
        params: { usuarioId }
    });
    return data;
}

export async function createAtivo(payload: { usuarioId: number; ticker: string; nome: string }) {
    const { data } = await api.post(`/investimentos`, payload);
    return data;
}

export async function listAtivos(usuarioId: number) {
    const cards = await fetchInvestmentCards(usuarioId);
    return cards.map(c => ({ id: c.ativoId, nome: c.nome, ticker: c.ticker }));
}
