<template>
  <div class="rounded-2xl border p-4 bg-white/5 backdrop-blur">
    <div class="flex items-center justify-between">
      <div class="flex flex-col">
        <span class="text-sm opacity-70">{{ card.ticker }}</span>
        <span class="text-lg font-semibold">{{ card.nome }}</span>
      </div>
      <div
          class="text-xl font-bold"
          :class="card.percentual >= 0 ? 'text-emerald-600' : 'text-red-600'">
        {{ pct(card.percentual) }}
      </div>
    </div>

    <div class="mt-3 grid grid-cols-2 gap-2 text-sm">
      <div class="rounded-xl bg-black/5 p-3">
        <div class="opacity-60">Quantidade</div>
        <div class="font-medium">{{ n(card.quantidade) }}</div>
      </div>
      <div class="rounded-xl bg-black/5 p-3">
        <div class="opacity-60">Preço Médio</div>
        <div class="font-medium">R$ {{ money(card.precoMedio) }}</div>
      </div>
      <div class="rounded-xl bg-black/5 p-3">
        <div class="opacity-60">Valor Atual</div>
        <div class="font-medium">R$ {{ money(card.valorAtual) }}</div>
      </div>
      <div class="rounded-xl bg-black/5 p-3">
        <div class="opacity-60">Total Investido</div>
        <div class="font-medium">R$ {{ money(card.totalInvestido) }}</div>
      </div>
      <div class="rounded-xl bg-black/5 p-3 col-span-2">
        <div class="opacity-60">Ganho Não Realizado</div>
        <div class="font-medium"
             :class="card.ganhoNaoRealizado >= 0 ? 'text-emerald-600' : 'text-red-600'">
          R$ {{ money(card.ganhoNaoRealizado) }}
        </div>
      </div>
      <div class="text-xs opacity-60 col-span-2">
        {{'Cotação atual' }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { InvestCard } from '../api/investments';
defineProps<{ card: InvestCard }>();

const n = (v: number) => new Intl.NumberFormat('pt-BR', { maximumFractionDigits: 4 }).format(v);
const money = (v: number) => new Intl.NumberFormat('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(v);
const pct = (v: number) => `${v >= 0 ? '+' : ''}${new Intl.NumberFormat('pt-BR', { maximumFractionDigits: 2 }).format(v)}%`;
</script>
