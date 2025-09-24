<template>
  <div class="p-4 space-y-4">
    <header class="flex items-center justify-between">
      <h1 class="text-xl font-semibold">Investimentos</h1>
      <button class="px-3 py-2 rounded-xl bg-blue-600 text-white" @click="open = true">
        Novo ativo
      </button>
    </header>

    <div v-if="store.loading" class="text-sm opacity-80">Carregando...</div>
    <div v-else-if="store.error" class="text-sm text-red-500">{{ store.error }}</div>

    <div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      <InvestmentCard v-for="c in store.cards" :key="c.ativoId" :card="c" />
    </div>

    <!-- Modal simples -->
    <dialog ref="dlg" class="rounded-2xl p-0 w-full max-w-md" :open="open" @close="open=false">
      <form class="p-4 space-y-3" @submit.prevent="submit">
        <h2 class="text-lg font-medium">Novo ativo</h2>
        <div class="space-y-1">
          <label class="text-sm">Ticker (ex: PETR4.SA)</label>
          <input v-model="ticker" class="w-full rounded-xl border px-3 py-2 bg-white/5" required />
        </div>
        <div class="space-y-1">
          <label class="text-sm">Nome</label>
          <input v-model="nome" class="w-full rounded-xl border px-3 py-2 bg-white/5" required />
        </div>
        <div class="flex gap-2 justify-end pt-2">
          <button type="button" class="px-3 py-2 rounded-xl border" @click="open=false">Cancelar</button>
          <button class="px-3 py-2 rounded-xl bg-blue-600 text-white">Salvar</button>
        </div>
      </form>
    </dialog>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
import { useInvestmentsStore } from '../stores/investments';
import InvestmentCard from '../components/InvestmentCard.vue';

// ajuste: pegue usuarioId do auth store/jwt. Temporário fixo:
const usuarioId = 1;

const store = useInvestmentsStore();
const open = ref(false);
const dlg = ref<HTMLDialogElement | null>(null);
const ticker = ref('');
const nome = ref('');

onMounted(async () => {
  await store.loadCards(usuarioId);
  await store.refreshAtivos(usuarioId);
});

watch(open, (v) => v && dlg.value?.showModal());

async function submit() {
  await store.addAtivo(usuarioId, ticker.value.trim(), nome.value.trim());
  ticker.value = ''; nome.value = '';
  open.value = false; dlg.value?.close();
}
</script>

<style scoped>
dialog::backdrop { background: rgba(0,0,0,.3); }
</style>
