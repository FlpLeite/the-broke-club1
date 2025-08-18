import { defineStore } from 'pinia';
import { fetchInvestmentCards, createAtivo, type InvestCard, listAtivos } from '../api/investments.ts';

export const useInvestmentsStore = defineStore('investments', {
    state: () => ({
        cards: [] as InvestCard[],
        ativosLite: [] as { id: number; nome: string; ticker: string }[],
        loading: false,
        error: '' as string | '',
    }),
    actions: {
        async loadCards(usuarioId: number) {
            this.loading = true; this.error = '';
            try {
                this.cards = await fetchInvestmentCards(usuarioId);
            } catch (e: any) {
                this.error = e?.message ?? 'Erro ao carregar investimentos';
            } finally {
                this.loading = false;
            }
        },
        async refreshAtivos(usuarioId: number) {
            try {
                this.ativosLite = await listAtivos(usuarioId);
            } catch { /* silencioso */ }
        },
        async addAtivo(usuarioId: number, ticker: string, nome: string) {
            await createAtivo({ usuarioId, ticker, nome });
            await Promise.all([this.loadCards(usuarioId), this.refreshAtivos(usuarioId)]);
        },
    },
});
