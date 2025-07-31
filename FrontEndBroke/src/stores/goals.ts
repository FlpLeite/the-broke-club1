import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'
import { useAuthStore } from './auth'

export interface Goal {
  id: string
  titulo: string
  categoria: string
  valorMeta: number
  valorAtual: number
  dataLimite: string
  descricao?: string
  criadoEm: string
  progresso?: number
}

type CreateGoalPayload = Omit<Goal, 'id'|'criadoEm'|'valorAtual'|'progresso'>
type UpdateGoalPayload = Partial<Omit<Goal, 'id'|'criadoEm'|'progresso'>>

export const useGoalsStore = defineStore('goals', () => {
  const authStore = useAuthStore()
  const userId = computed(() => authStore.user?.idUsuario)
  const goals = ref<Goal[]>([])

  async function loadGoals() {
    if (!userId.value) return
    const res = await axios.get(`http://localhost:5024/metas/usuario/${userId.value}`)
    goals.value = res.data.map((m: any) => ({
      id:         m.idObjetivo.toString(),  
      titulo:     m.titulo,                 
      categoria:  m.categoria,             
      valorMeta:  Number(m.valorMeta),      
      valorAtual: Number(m.valorAtual),     
      dataLimite: m.dataLimite.slice(0,10),  
      descricao:  m.descricao,
      criadoEm:   m.dataCriacao ?? '',       
      progresso:  m.valorMeta > 0
                  ? (Number(m.valorAtual)/Number(m.valorMeta))*100
                  : 0
    }))
  }

  async function addGoal(payload: CreateGoalPayload) {
    if (!userId.value) return
    await axios.post('http://localhost:5024/metas', {
      idUsuario:  userId.value,          
      titulo:     payload.titulo,
      categoria:  payload.categoria,
      valorMeta:  payload.valorMeta,     
      valorAtual: 0,
      dataLimite: new Date(payload.dataLimite).toISOString(),   
      descricao:  payload.descricao
    })
    await loadGoals()
  }

  async function updateGoal(id: string, payload: UpdateGoalPayload) {
    await axios.put(`http://localhost:5024/metas/${id}`, {
      idObjetivo: Number(id),            
      titulo:     payload.titulo,
      categoria:  payload.categoria,
      valorMeta:  payload.valorMeta,
      valorAtual: payload.valorAtual,
      dataLimite: new Date(payload.dataLimite!).toISOString(),
      descricao:  payload.descricao
    })
    await loadGoals()
  }

  async function deleteGoal(id: string) {
    await axios.delete(`http://localhost:5024/metas/${id}`)
    await loadGoals()
  }
  async function updateGoalProgress(id: string, amount: number | string) {
    const valor = typeof amount === 'string'
      ? parseFloat(amount.replace(',', '.'))
      : amount;
  
    await axios.patch(
      `http://localhost:5024/metas/${id}/progresso`,
      { valor }
    );
  
    await loadGoals();
  }

  return {
    goals,
    goalProgress: computed(() => goals.value),
    loadGoals,
    addGoal,
    updateGoal,
    deleteGoal,
    updateGoalProgress
  }
})
