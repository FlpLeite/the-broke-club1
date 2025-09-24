<template>
  <div class="relative">
    <label v-if="label" class="text-sm mb-1 block">{{ label }}</label>
    <input
        :placeholder="placeholder"
        v-model="query"
        @focus="open = true"
        @keydown.down.prevent="move(+1)"
        @keydown.up.prevent="move(-1)"
        @keydown.enter.prevent="choose()"
        class="w-full rounded-xl border px-3 py-2 bg-white"
    />

    <ul v-if="open && results.length" class="absolute z-20 mt-1 w-full max-h-64 overflow-auto rounded-xl border bg-white shadow">
      <li v-for="(it, i) in results" :key="it.symbol"
          @mousedown.prevent="select(it)"
          :class="['px-3 py-2 cursor-pointer', i===hi ? 'bg-gray-100' : '']">
        <div class="text-sm font-medium">{{ it.symbol }}</div>
        <div class="text-xs text-gray-500">{{ it.name }} <span v-if="it.region">· {{ it.region }}</span></div>
      </li>
    </ul>

    <p v-if="open && query.length>=3 && loading" class="text-xs mt-1 text-gray-500">buscando…</p>
    <p v-if="open && query.length>=3 && !loading && !results.length" class="text-xs mt-1 text-gray-500">sem resultados</p>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted, onBeforeUnmount } from 'vue';
import { searchSymbols, type SymbolItem } from '../api/investments.ts';

const props = defineProps<{ modelValue: string; label?: string; placeholder?: string }>();
const emit = defineEmits<{ (e:'update:modelValue', v:string):void; (e:'select', item:SymbolItem):void }>();

const query = ref(props.modelValue || '');
const results = ref<SymbolItem[]>([]);
const loading = ref(false);
const open = ref(false);
const hi = ref(-1);
let timer: number | undefined;

watch(() => props.modelValue, v => { if (v !== query.value) query.value = v; });

watch(query, (q) => {
  emit('update:modelValue', q);
  if (timer) clearTimeout(timer);
  if (!q || q.length < 3) { results.value = []; hi.value = -1; return; }
  loading.value = true;
  timer = window.setTimeout(async () => {
    try {
      results.value = await searchSymbols(q);
      hi.value = results.value.length ? 0 : -1;
    } finally {
      loading.value = false;
    }
  }, 400); // debounce
});

function select(item: SymbolItem) {
  query.value = item.symbol;
  emit('update:modelValue', item.symbol);
  emit('select', item);
  open.value = false;
}
function move(delta: number) {
  if (!results.value.length) return;
  hi.value = (hi.value + delta + results.value.length) % results.value.length;
}
function choose() { if (hi.value >= 0) select(results.value[hi.value]); }

function onDocClick(e: MouseEvent) {
  const el = e.target as HTMLElement;
  if (!el.closest('.relative')) open.value = false;
}
onMounted(() => document.addEventListener('click', onDocClick));
onBeforeUnmount(() => document.removeEventListener('click', onDocClick));
</script>
