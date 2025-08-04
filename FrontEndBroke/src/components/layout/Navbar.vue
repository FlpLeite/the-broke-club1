<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth'

const router = useRouter()
const authStore = useAuthStore()
const isAuthenticated = computed(() => authStore.isAuthenticated)
const mobileMenuOpen = ref(false)
const isLight = ref(true)

const toggleMobileMenu = () => {
  mobileMenuOpen.value = !mobileMenuOpen.value
}

const onThemeChange = () => {
  if (!isLight.value) {
    document.documentElement.classList.add('dark')
    localStorage.setItem('theme', 'dark')
  } else {
    document.documentElement.classList.remove('dark')
    localStorage.setItem('theme', 'light')
  }
}

onMounted(() => {
  isLight.value = localStorage.getItem('theme') !== 'dark'
  if (!isLight.value) {
    document.documentElement.classList.add('dark')
  }
})

const logout = () => {
  authStore.logout()
  router.push('/')
}
</script>

<template>
  <nav class="bg-white shadow-md dark:bg-[#121C2A]">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between h-16">
        <div class="flex">
          <div class="flex-shrink-0 flex items-center">
            <router-link to="/" class="text-xl font-bold text-primary-600">

              The Broke Club
            </router-link>
          </div>
          <div class="hidden sm:ml-6 sm:flex sm:space-x-8">
            <router-link
                v-if="!isAuthenticated"
                to="/"
                class="inline-flex items-center px-1 pt-1 border-b-2 border-transparent text-sm font-medium text-gray-500 hover:text-gray-700 hover:border-gray-300">
              Início
            </router-link>
            <template v-if="isAuthenticated">
              <router-link to="/dashboard" class="inline-flex items-center px-1 pt-1 border-b-2 border-transparent text-sm font-medium text-gray-500 hover:text-gray-700 hover:border-gray-300">
                Painel
              </router-link>
              <router-link to="/transactions" class="inline-flex items-center px-1 pt-1 border-b-2 border-transparent text-sm font-medium text-gray-500 hover:text-gray-700 hover:border-gray-300">
                Transações
              </router-link>
              <router-link to="/investiments" class="inline-flex items-center px-1 pt-1 border-b-2 border-transparent text-sm font-medium text-gray-500 hover:text-gray-700 hover:border-gray-300">
                Investimentos
              </router-link>
              <router-link to="/goals" class="inline-flex items-center px-1 pt-1 border-b-2 border-transparent text-sm font-medium text-gray-500 hover:text-gray-700 hover:border-gray-300">
                Metas
              </router-link>
              <router-link to="/financial-analysis" class="inline-flex items-center px-1 pt-1 border-b-2 border-transparent text-sm font-medium text-gray-500 hover:text-gray-700 hover:border-gray-300">
                IA
              </router-link>
            </template>
          </div>
        </div>
        <div class="hidden sm:ml-6 sm:flex sm:items-center">
          <template v-if="isAuthenticated">
            <button @click="logout" class="ml-3 btn btn-secondary">
              Sair
            </button>
          </template>
          <template v-else>
            <router-link to="/login" class="btn btn-secondary mr-2">
              Entrar
            </router-link>
            <router-link to="/signup" class="btn btn-primary">
              Cadastrar
            </router-link>
          </template>
          <label class="ml-3 switch">
            <input
                type="checkbox"
                class="circle"
                v-model="isLight"
                @change="onThemeChange"
            />
            <svg
                viewBox="0 0 384 512"
                xmlns="http://www.w3.org/2000/svg"
                class="moon svg"
            >
              <path
                  d="M223.5 32C100 32 0 132.3 0 256S100 480 223.5 480c60.6 0 115.5-24.2 155.8-63.4c5-4.9 6.3-12.5 3.1-18.7s-10.1-9.7-17-8.5c-9.8 1.7-19.8 2.6-30.1 2.6c-96.9 0-175.5-78.8-175.5-176c0-65.8 36-123.1 89.3-153.3c6.1-3.5 9.2-10.5 7.7-17.3s-7.3-11.9-14.3-12.5c-6.3-.5-12.6-.8-19-.8z"
              ></path>
            </svg>
            <div class="sun svg">
              <span class="dot"></span>
            </div>
          </label>
        </div>
        <div class="-mr-2 flex items-center sm:hidden">
          <button @click="toggleMobileMenu" class="inline-flex items-center justify-center p-2 rounded-md text-gray-400 hover:text-gray-500 hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-primary-500">
            <span class="sr-only">Abrir menu principal</span>
            <svg class="h-6 w-6" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor" aria-hidden="true">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
            </svg>
          </button>
        </div>
      </div>
    </div>

    <div v-if="mobileMenuOpen" class="sm:hidden">
      <div class="pt-2 pb-3 space-y-1">
        <router-link to="/" class="block pl-3 pr-4 py-2 border-l-4 border-transparent text-base font-medium text-gray-600 hover:bg-gray-50 hover:border-gray-300 hover:text-gray-800">
          Início
        </router-link>
        <template v-if="isAuthenticated">
          <router-link to="/dashboard" class="block pl-3 pr-4 py-2 border-l-4 border-transparent text-base font-medium text-gray-600 hover:bg-gray-50 hover:border-gray-300 hover:text-gray-800">
            Painel
          </router-link>
          <router-link to="/transactions" class="block pl-3 pr-4 py-2 border-l-4 border-transparent text-base font-medium text-gray-600 hover:bg-gray-50 hover:border-gray-300 hover:text-gray-800">
            Transações
          </router-link>
        </template>
      </div>
      <div class="pt-4 pb-3 border-t border-gray-200">
        <div class="flex items-center px-4">
          <template v-if="isAuthenticated">
            <button @click="logout" class="block w-full text-left px-4 py-2 text-base font-medium text-gray-500 hover:text-gray-800 hover:bg-gray-100">
              Sair
            </button>
          </template>
          <template v-else>
            <div class="flex flex-col space-y-2 w-full px-4 py-2">
              <router-link to="/login" class="btn btn-secondary text-center">
                Entrar
              </router-link>
              <router-link to="/signup" class="btn btn-primary text-center">
                Cadastrar
              </router-link>
            </div>
          </template>
          <label class="mt-2 switch">
            <input
                type="checkbox"
                class="circle"
                v-model="isLight"
                @change="onThemeChange"
            />
            <svg
                viewBox="0 0 384 512"
                xmlns="http://www.w3.org/2000/svg"
                class="moon svg"
            >
              <path
                  d="M223.5 32C100 32 0 132.3 0 256S100 480 223.5 480c60.6 0 115.5-24.2 155.8-63.4c5-4.9 6.3-12.5 3.1-18.7s-10.1-9.7-17-8.5c-9.8 1.7-19.8 2.6-30.1 2.6c-96.9 0-175.5-78.8-175.5-176c0-65.8 36-123.1 89.3-153.3c6.1-3.5 9.2-10.5 7.7-17.3s-7.3-11.9-14.3-12.5c-6.3-.5-12.6-.8-19-.8z"
              ></path>
            </svg>
            <div class="sun svg">
              <span class="dot"></span>
            </div>
          </label>
        </div>
      </div>
    </div>
  </nav>
</template>