import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import Dashboard from '../views/Dashboard.vue'
import Transactions from '../views/Transactions.vue'
import Login from '../views/auth/Login.vue'
import Signup from '../views/auth/Signup.vue'
import { useAuthStore } from '../stores/auth'
import Investiments from '../views/Investiments.vue'
import Goals from '../views/Goals.vue'
import FinancialAnalysis from '../views/FinancialAnalysis.vue' // Componente direto em /views

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
      path: '/login',
      name: 'login',
      component: Login
    },
    {
      path: '/signup',
      name: 'signup',
      component: Signup
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: Dashboard,
      meta: { requiresAuth: true }
    },
    {
      path: '/transactions',
      name: 'transactions',
      component: Transactions,
      meta: { requiresAuth: true }
    },
    {
      path: '/investiments',
      name: 'investiments',
      component: Investiments,
      meta: { requiresAuth: true }
    },
    {
      path: '/goals',
      name: 'goals',
      component: Goals,
      meta: { requiresAuth: true }
    },
    {
      path: '/financial-analysis',
      name: 'financial-analysis',
      component: FinancialAnalysis,
      meta: { 
        requiresAuth: true,
        title: 'Análise Financeira IA' // Título para exibição
      }
    }
  ]
})

router.beforeEach((to, _from, next) => {
  const authStore = useAuthStore()
  
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next({ name: 'login' })
  } else {
    next()
  }
})

// Opcional: Definir título da página dinamicamente
router.afterEach((to) => {
  const title = to.meta.title || 'Meu App Financeiro'
  document.title = `${title} | Meu App`
})

export default router