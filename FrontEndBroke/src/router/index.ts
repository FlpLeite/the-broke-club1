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
      meta: {
        requiresAuth: true
      }
    },
    {
      path: '/transactions',
      name: 'transactions',
      component: Transactions,
      meta: {
        requiresAuth: true,
        title: 'Transações'
      }
    },
    {
      path: '/investiments',
      name: 'investiments',
      component: Investiments,
      meta: {
        requiresAuth: true,
        title: 'Investimentos'
      }
    },
    {
      path: '/goals',
      name: 'goals',
      component: Goals,
      meta: {
        requiresAuth: true,
        title: 'Metas'
      }
    },
    {
      path: '/financial-analysis',
      name: 'financial-analysis',
      component: FinancialAnalysis,
      meta: { 
        requiresAuth: true,
        title: 'Análise Financeira IA'
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

router.afterEach((to) => {
  const title = to.meta.title || 'The broke club'
  document.title = `${title}`
})

export default router