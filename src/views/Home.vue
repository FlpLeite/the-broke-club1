<template>
  <div class="bg-gradient-to-b from-primary-50 to-white">
    <!-- Hero Section -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16 md:py-24">
      <div class="grid grid-cols-1 md:grid-cols-2 gap-8 items-center">
        <div>
          <h1 class="text-4xl md:text-5xl font-bold text-gray-900 mb-4">
            Assuma o Controle das Suas Finanças
          </h1>
          <p class="text-lg text-gray-600 mb-8">
            O Controlador de Investimentos ajuda você a acompanhar suas receitas e despesas, gerenciar seu orçamento e alcançar seus objetivos financeiros.
          </p>
          <div class="flex flex-col sm:flex-row space-y-4 sm:space-y-0 sm:space-x-4">
            <router-link to="/signup" class="btn btn-primary text-center py-3 px-6 text-lg">
              Começar Agora
            </router-link>
            <router-link to="/login" class="btn btn-secondary text-center py-3 px-6 text-lg">
              Entrar
            </router-link>
          </div>
        </div>
        <div class="hidden md:block">
          <div class="bg-primary-100 rounded-lg h-80 flex items-center justify-center">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-32 w-32 text-primary-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
        </div>
      </div>
    </div>

    <!-- Features Section -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
      <h2 class="text-3xl font-bold text-center text-gray-900 mb-12">Principais Recursos</h2>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
        <div class="bg-white p-6 rounded-lg shadow-md">
          <div class="text-primary-500 mb-4">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z" />
            </svg>
          </div>
          <h3 class="text-xl font-semibold mb-2">Acompanhe Transações</h3>
          <p class="text-gray-600">
            Registre e categorize facilmente suas receitas e despesas para acompanhar para onde seu dinheiro está indo.
          </p>
        </div>
        <div class="bg-white p-6 rounded-lg shadow-md">
          <div class="text-primary-500 mb-4">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 3.055A9.001 9.001 0 1020.945 13H11V3.055z" />
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.488 9H15V3.512A9.025 9.025 0 0120.488 9z" />
            </svg>
          </div>
          <h3 class="text-xl font-semibold mb-2">Visualize Dados </h3>
          <p class="text-gray-600">
            Veja seus dados financeiros em gráficos claros e intuitivos para entender melhor seus hábitos de gastos.
          </p>
        </div>
        <div class="bg-white p-6 rounded-lg shadow-md">
          <div class="text-primary-500 mb-4">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          <h3 class="text-xl font-semibold mb-2">Atualizações em Tempo Real</h3>
          <p class="text-gray-600">
            Receba atualizações instantâneas sobre seu status financeiro conforme adiciona novas transações e gerencia suas contas.
          </p>
        </div>
      </div>
    </div>

    <!-- CTA Section -->
    <div class="bg-primary-600 py-16">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
        <h2 class="text-3xl font-bold text-white mb-4">Pronto para Assumir o Controle das Suas Finanças?</h2>
        <p class="text-xl text-primary-100 mb-8">
          Junte-se a milhares de usuários que já estão gerenciando suas finanças com o Controlador de Investimentos.
        </p>
        <router-link to="/signup" class="inline-block btn bg-white text-primary-600 hover:bg-gray-100 py-3 px-8 text-lg font-medium">
          Cadastre-se Agora
        </router-link>
      </div>
    </div>
  </div>
</template>