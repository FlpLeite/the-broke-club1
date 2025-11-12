  <template>
    <div class="min-h-screen bg-gradient-to-br from-white via-primary-50 to-primary-100 dark:from-gray-900 dark:via-gray-800 dark:to-gray-900 overflow-hidden">
      <!-- Background Animated Elements -->
      <div class="fixed inset-0 overflow-hidden pointer-events-none">
        <div class="absolute -top-40 -right-40 w-80 h-80 bg-primary-200 dark:bg-primary-800 rounded-full blur-3xl opacity-30 animate-float-slow"></div>
        <div class="absolute top-1/2 -left-20 w-60 h-60 bg-yellow-200 dark:bg-yellow-800 rounded-full blur-3xl opacity-20 animate-float-medium"></div>
        <div class="absolute -bottom-20 left-1/3 w-72 h-72 bg-green-200 dark:bg-green-800 rounded-full blur-3xl opacity-25 animate-float-slow"></div>
        
        <!-- Grid Pattern -->
        <div class="absolute inset-0 opacity-5 dark:opacity-10">
          <div class="grid-pattern"></div>
        </div>
      </div>

      <!-- Header/Hero Section -->
      <header class="relative pt-20 pb-16 md:pt-32 md:pb-24">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
            <!-- Text Content -->
            <div class="relative z-10">
              <!-- Animated Badge -->
              <div class="inline-flex items-center px-4 py-2 rounded-full text-sm font-medium bg-white/80 dark:bg-gray-800/80 backdrop-blur-sm text-primary-700 dark:text-primary-300 border border-primary-200 dark:border-primary-700 shadow-lg mb-8 animate-slide-in-left">
                <div class="w-2 h-2 bg-primary-500 rounded-full mr-2 animate-pulse"></div>
                ✨ Feito para quem quer sair do "zero a zero" no fim do mês
              </div>

              <h1 class="text-4xl md:text-5xl lg:text-6xl font-black text-gray-900 dark:text-white mb-6 leading-tight">
                <span class="block animate-typing overflow-hidden whitespace-nowrap border-r-4 border-primary-500 pr-2">
                  CHEGA DE DIZER
                </span>
                <span class="text-transparent bg-clip-text bg-gradient-to-r from-primary-600 to-primary-400 dark:from-primary-400 dark:to-primary-300 animate-gradient-x">
                  "NÃO SOBROU NADA"
                </span>
              </h1>
              
              <p class="text-xl text-gray-600 dark:text-gray-300 mb-9 max-w-lg leading-relaxed animate-fade-in-up">
                O <span class="font-semibold text-primary-600 dark:text-primary-400">The Broke Club</span> é a ferramenta que faltava para você finalmente entender para onde vai cada real e começar a construir sua liberdade financeira.
              </p>

              <!-- CTA Buttons -->
              <div class="flex flex-col sm:flex-row space-y-4 sm:space-y-0 sm:space-x-4 mb-8 animate-fade-in-up" style="animation-delay: 0.2s">
                <router-link 
                  to="/signup" 
                  class="group relative bg-gradient-to-r from-primary-600 to-primary-500 hover:from-primary-700 hover:to-primary-600 text-white text-center py-4 px-8 text-lg font-semibold rounded-xl shadow-lg hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 hover:scale-105 overflow-hidden"
                >
                  <span class="relative z-10">Quero Me Organizar - É Grátis</span>
                  <div class="absolute inset-0 bg-gradient-to-r from-white/20 to-transparent transform -skew-x-12 -translate-x-full group-hover:translate-x-full transition-transform duration-700"></div>
                </router-link>
                
                <a 
                  href="#features" 
                  class="group border-2 border-primary-600 dark:border-primary-400 text-primary-600 dark:text-primary-400 hover:bg-primary-600 dark:hover:bg-primary-400 hover:text-white text-center py-4 px-8 text-lg font-medium rounded-xl transition-all duration-300 transform hover:-translate-y-1"
                >
                  <span class="flex items-center justify-center">
                    Como Funciona
                    <svg class="w-5 h-5 ml-2 transform group-hover:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 14l-7 7m0 0l-7-7m7 7V3"></path>
                    </svg>
                  </span>
                </a>
              </div>

              <!-- Trust Indicators -->
              <div class="flex flex-wrap items-center gap-6 text-sm text-gray-500 dark:text-gray-400 animate-fade-in-up" style="animation-delay: 0.4s">
                <div class="flex items-center">
                  <svg class="w-4 h-4 text-green-500 mr-1" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                  </svg>
                  <span>Sem planos complicados</span>
                </div>
                <div class="flex items-center">
                  <svg class="w-4 h-4 text-green-500 mr-1" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                  </svg>
                  <span>Interface simples e direta</span>
                </div>
                <div class="flex items-center">
                  <svg class="w-4 h-4 text-green-500 mr-1" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                  </svg>
                  <span>Feito para brasileiros</span>
                </div>
              </div>
            </div>

            <!-- Visual Content -->
            <div class="relative animate-float-medium">
              <!-- Main Card -->
              <div class="relative bg-white/80 dark:bg-gray-800/80 backdrop-blur-sm rounded-3xl p-6 shadow-2xl border border-white/20 dark:border-gray-700/20 transform rotate-2 hover:rotate-0 transition-transform duration-500">
                <div class="bg-gradient-to-br from-primary-500 to-primary-600 rounded-2xl p-1">
                  <div class="bg-white dark:bg-gray-800 rounded-xl overflow-hidden">
                    <img src="../assets/logoBrokeDark2.png" alt="The Broke Club Dashboard" class="w-full h-auto">
                  </div>
                </div>
                
                <!-- Floating Elements -->
                <div class="absolute -top-4 -left-4 w-20 h-20 bg-yellow-400 rounded-2xl rotate-12 shadow-lg animate-bounce-slow">
                  <div class="w-full h-full flex items-center justify-center text-yellow-800 font-bold text-sm">💰</div>
                </div>
                
                <div class="absolute -bottom-4 -right-4 w-16 h-16 bg-green-400 rounded-2xl -rotate-12 shadow-lg animate-bounce-slow" style="animation-delay: 1s">
                  <div class="w-full h-full flex items-center justify-center text-green-800 font-bold text-sm">📈</div>
                </div>
              </div>

              <!-- Background Glow -->
              <div class="absolute inset-0 bg-gradient-to-r from-primary-400/20 to-primary-600/20 rounded-3xl blur-xl -z-10 animate-pulse"></div>
            </div>
          </div>
        </div>
      </header>

      <!-- Problem Section -->
      <section class="relative py-20 bg-white/50 dark:bg-gray-800/50 backdrop-blur-sm">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="text-center mb-16">
            <h2 class="text-3xl md:text-4xl font-black text-gray-900 dark:text-white mb-6">
              Você se identifica com alguma dessas situações?
            </h2>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            <div class="bg-red-50 dark:bg-red-900/20 p-6 rounded-2xl border border-red-200 dark:border-red-800 text-center">
              <div class="text-3xl mb-4">😅</div>
              <h3 class="font-bold text-gray-900 dark:text-white mb-2">Saldo Zerado</h3>
              <p class="text-gray-600 dark:text-gray-300 text-sm">Mês acaba e você não sabe pra onde foi o dinheiro</p>
            </div>

            <div class="bg-orange-50 dark:bg-orange-900/20 p-6 rounded-2xl border border-orange-200 dark:border-orange-800 text-center">
              <div class="text-3xl mb-4">📱</div>
              <h3 class="font-bold text-gray-900 dark:text-white mb-2">App Complexo</h3>
              <p class="text-gray-600 dark:text-gray-300 text-sm">Tentou outros apps mas desistiu pela complexidade</p>
            </div>

            <div class="bg-blue-50 dark:bg-blue-900/20 p-6 rounded-2xl border border-blue-200 dark:border-blue-800 text-center">
              <div class="text-3xl mb-4">🎯</div>
              <h3 class="font-bold text-gray-900 dark:text-white mb-2">Sem Metas</h3>
              <p class="text-gray-600 dark:text-gray-300 text-sm">Queria economizar mas não sabe por onde começar</p>
            </div>

            <div class="bg-purple-50 dark:bg-purple-900/20 p-6 rounded-2xl border border-purple-200 dark:border-purple-800 text-center">
              <div class="text-3xl mb-4">💸</div>
              <h3 class="font-bold text-gray-900 dark:text-white mb-2">Gastos Invisíveis</h3>
              <p class="text-gray-600 dark:text-gray-300 text-sm">Pequenos gastos que somados fazem diferença</p>
            </div>
          </div>

          <div class="text-center mt-12 bg-primary-50 dark:bg-primary-900/20 rounded-2xl p-8 border border-primary-200 dark:border-primary-800">
            <h3 class="text-2xl font-bold text-gray-900 dark:text-white mb-4">
              Se identificou? <span class="text-primary-600 dark:text-primary-400">Calma, tem solução!</span>
            </h3>
            <p class="text-gray-600 dark:text-gray-300 max-w-2xl mx-auto">
              O The Broke Club foi criado exatamente para resolver esses problemas de forma simples e direta, 
              sem complicação e com uma linguagem que todo mundo entende.
            </p>
          </div>
        </div>
      </section>

      <!-- Features Section -->
      <section id="features" class="relative py-20">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="text-center mb-20">
            <h2 class="text-4xl md:text-5xl font-black text-gray-900 dark:text-white mb-6">
              Como o <span class="text-transparent bg-clip-text bg-gradient-to-r from-primary-600 to-primary-400 dark:from-primary-400 dark:to-primary-300">The Broke Club</span><br>vai te ajudar?
            </h2>
            <p class="text-xl text-gray-600 dark:text-gray-300 max-w-3xl mx-auto leading-relaxed">
              Ferramentas simples que fazem a diferença no seu dia a dia financeiro
            </p>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
            <!-- Feature 1 -->
            <div class="group relative bg-white/70 dark:bg-gray-800/70 backdrop-blur-sm p-8 rounded-3xl shadow-lg hover:shadow-2xl border border-white/20 dark:border-gray-700/20 transition-all duration-500 transform hover:-translate-y-4 animate-fade-in-up">
              <div class="absolute inset-0 bg-gradient-to-br from-primary-500/5 to-transparent rounded-3xl opacity-0 group-hover:opacity-100 transition-opacity duration-500"></div>
              
              <div class="relative z-10">
                <div class="bg-gradient-to-br from-primary-500 to-primary-600 w-20 h-20 rounded-2xl flex items-center justify-center mb-6 transform group-hover:scale-110 group-hover:rotate-6 transition-transform duration-500 shadow-lg">
                  <span class="text-2xl text-white">📝</span>
                </div>
                
                <h3 class="text-2xl font-bold mb-4 text-gray-900 dark:text-white group-hover:text-primary-600 dark:group-hover:text-primary-400 transition-colors">
                  Anotação Rápida
                </h3>
                
                <p class="text-gray-600 dark:text-gray-300 leading-relaxed mb-6">
                  Registre ganhos e gastos em segundos. Interface tão simples que você vai usar até no meio do rolê.
                </p>
                
                <ul class="space-y-2 text-gray-500 dark:text-gray-400">
                  <li class="flex items-center">
                    <svg class="w-5 h-5 text-green-500 mr-2" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                    </svg>
                    Leva menos de 10 segundos
                  </li>
                  <li class="flex items-center">
                    <svg class="w-5 h-5 text-green-500 mr-2" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                    </svg>
                    Categorias inteligentes
                  </li>
                </ul>
              </div>
            </div>

            <!-- Feature 2 -->
            <div class="group relative bg-white/70 dark:bg-gray-800/70 backdrop-blur-sm p-8 rounded-3xl shadow-lg hover:shadow-2xl border border-white/20 dark:border-gray-700/20 transition-all duration-500 transform hover:-translate-y-4 animate-fade-in-up" style="animation-delay: 0.1s">
              <div class="absolute inset-0 bg-gradient-to-br from-primary-500/5 to-transparent rounded-3xl opacity-0 group-hover:opacity-100 transition-opacity duration-500"></div>
              
              <div class="relative z-10">
                <div class="bg-gradient-to-br from-primary-500 to-primary-600 w-20 h-20 rounded-2xl flex items-center justify-center mb-6 transform group-hover:scale-110 group-hover:rotate-6 transition-transform duration-500 shadow-lg">
                  <span class="text-2xl text-white">📊</span>
                </div>
                
                <h3 class="text-2xl font-bold mb-4 text-gray-900 dark:text-white group-hover:text-primary-600 dark:group-hover:text-primary-400 transition-colors">
                  Visualização Clara
                </h3>
                
                <p class="text-gray-600 dark:text-gray-300 leading-relaxed mb-6">
                  Gráficos que você realmente entende. Veja na hora onde está indo seu dinheiro e tome decisões melhores.
                </p>
                
                <ul class="space-y-2 text-gray-500 dark:text-gray-400">
                  <li class="flex items-center">
                    <svg class="w-5 h-5 text-green-500 mr-2" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                    </svg>
                    Gráficos simples e diretos
                  </li>
                  <li class="flex items-center">
                    <svg class="w-5 h-5 text-green-500 mr-2" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                    </svg>
                    Identifique padrões facilmente
                  </li>
                </ul>
              </div>
            </div>

            <!-- Feature 3 -->
            <div class="group relative bg-white/70 dark:bg-gray-800/70 backdrop-blur-sm p-8 rounded-3xl shadow-lg hover:shadow-2xl border border-white/20 dark:border-gray-700/20 transition-all duration-500 transform hover:-translate-y-4 animate-fade-in-up" style="animation-delay: 0.2s">
              <div class="absolute inset-0 bg-gradient-to-br from-primary-500/5 to-transparent rounded-3xl opacity-0 group-hover:opacity-100 transition-opacity duration-500"></div>
              
              <div class="relative z-10">
                <div class="bg-gradient-to-br from-primary-500 to-primary-600 w-20 h-20 rounded-2xl flex items-center justify-center mb-6 transform group-hover:scale-110 group-hover:rotate-6 transition-transform duration-500 shadow-lg">
                  <span class="text-2xl text-white">🎯</span>
                </div>
                
                <h3 class="text-2xl font-bold mb-4 text-gray-900 dark:text-white group-hover:text-primary-600 dark:group-hover:text-primary-400 transition-colors">
                  Foco no Essencial
                </h3>
                
                <p class="text-gray-600 dark:text-gray-300 leading-relaxed mb-6">
                  Sem funcionalidades que você não usa. Apenas o necessário para você controlar suas finanças sem estresse.
                </p>
                
                <ul class="space-y-2 text-gray-500 dark:text-gray-400">
                  <li class="flex items-center">
                    <svg class="w-5 h-5 text-green-500 mr-2" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                    </svg>
                    Apenas o que importa
                  </li>
                  <li class="flex items-center">
                    <svg class="w-5 h-5 text-green-500 mr-2" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                    </svg>
                    Zero complicação
                  </li>
                </ul>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Story Section -->
      <section class="relative py-20 bg-gradient-to-br from-primary-500/5 to-primary-600/10 dark:from-primary-500/5 dark:to-primary-600/10">
        <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="text-center mb-12">
            <h2 class="text-3xl md:text-4xl font-black text-gray-900 dark:text-white mb-6">
              Por que criamos o <span class="text-primary-600 dark:text-primary-400">The Broke Club</span>?
            </h2>
          </div>

          <div class="bg-white/80 dark:bg-gray-800/80 backdrop-blur-sm rounded-3xl p-8 shadow-lg border border-white/20 dark:border-gray-700/20">
            <div class="prose prose-lg dark:prose-invert max-w-none">
              <p class="text-lg text-gray-600 dark:text-gray-300 mb-6 leading-relaxed">
                <span class="font-semibold text-primary-600 dark:text-primary-400">The Broke Club nasceu de uma necessidade real:</span> ver tantas pessoas inteligentes e trabalhadoras chegando no fim do mês sem entender para onde foi o dinheiro.
              </p>
              
              <p class="text-lg text-gray-600 dark:text-gray-300 mb-6 leading-relaxed">
                Percebemos que a maioria dos apps financeiros são ou muito complexos (cheios de funcionalidades que ninguém usa) ou muito básicos (não ajudam de verdade). 
                <span class="font-semibold">Faltava algo no meio termo:</span> uma ferramenta simples mas poderosa, feita especialmente para quem está começando a se organizar.
              </p>

              <div class="bg-primary-50 dark:bg-primary-900/20 rounded-2xl p-6 my-8 border border-primary-200 dark:border-primary-800">
                <p class="text-primary-700 dark:text-primary-300 font-semibold text-center">
                  💡 Desenvolvemos cada funcionalidade pensando em você que quer sair do "não sobrou nada" para o "sobra todo mês!"
                </p>
              </div>

              <p class="text-lg text-gray-600 dark:text-gray-300 leading-relaxed">
                Este projeto é feito com muito carinho e acreditamos que <span class="font-semibold">organização financeira deve ser acessível a todos</span>, 
                sem complicação e com uma dose de simpatia no processo.
              </p>
            </div>
          </div>
        </div>
      </section>

      <!-- AI Analysis Section -->
      <section class="relative py-20 bg-gradient-to-br from-white to-gray-50 dark:from-gray-800 dark:to-gray-900">
        <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="text-center mb-16">
            <h2 class="text-4xl md:text-5xl font-black text-gray-900 dark:text-white mb-6">
              Análise Financeira
              <span class="text-transparent bg-clip-text bg-gradient-to-r from-primary-600 to-primary-400 dark:from-primary-400 dark:to-primary-300">Inteligente</span>
            </h2>
            <p class="text-xl text-gray-600 dark:text-gray-300 max-w-2xl mx-auto leading-relaxed">
              Receba insights personalizados sobre seus gastos e ganhos com nossa IA especializada
            </p>
          </div>

          <div class="bg-white/80 dark:bg-gray-800/80 backdrop-blur-sm rounded-3xl shadow-2xl border border-white/20 dark:border-gray-700/20 overflow-hidden">
            <!-- Header -->
            <div class="bg-gradient-to-r from-primary-600 to-primary-500 px-8 py-6 text-white">
              <h3 class="text-2xl font-bold text-center">Nível de Análise</h3>
              <p class="text-center text-primary-100 mt-2">Escolha a profundidade da análise conforme suas necessidades</p>
            </div>

            <div class="p-8">
              <!-- Analysis Levels -->
              <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
                <!-- Bronze -->
                <div class="group relative bg-gradient-to-br from-amber-50 to-amber-100 dark:from-amber-900/20 dark:to-amber-800/20 border-2 border-amber-200 dark:border-amber-700 rounded-2xl p-6 text-center transition-all duration-300 hover:scale-105 hover:shadow-lg cursor-pointer">
                  <div class="absolute -top-3 left-1/2 transform -translate-x-1/2">
                    <span class="bg-amber-500 text-white px-4 py-1 rounded-full text-sm font-bold">BRONZE</span>
                  </div>
                  <div class="mt-4">
                    <h4 class="text-lg font-bold text-amber-800 dark:text-amber-200 mb-3">Análise Básica</h4>
                    <p class="text-amber-600 dark:text-amber-300 text-sm mb-4">Ideal para uma visão geral rápida</p>
                    <div class="space-y-2 text-xs text-amber-700 dark:text-amber-300">
                      <p class="flex items-center justify-center">
                        <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                        </svg>
                        Resumo executivo
                      </p>
                      <p class="flex items-center justify-center">
                        <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                        </svg>
                        Principais observações
                      </p>
                      <p class="flex items-center justify-center">
                        <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                        </svg>
                        Dica prática rápida
                      </p>
                    </div>
                  </div>
                </div>

                <!-- Prata -->
                <div class="group relative bg-gradient-to-br from-gray-50 to-gray-100 dark:from-gray-800/20 dark:to-gray-700/20 border-2 border-gray-300 dark:border-gray-600 rounded-2xl p-6 text-center transition-all duration-300 hover:scale-105 hover:shadow-lg cursor-pointer transform hover:-translate-y-2">
                  <div class="absolute -top-3 left-1/2 transform -translate-x-1/2">
                    <span class="bg-gray-600 text-white px-4 py-1 rounded-full text-sm font-bold">PRATA</span>
                  </div>
                  <div class="mt-4">
                    <h4 class="text-lg font-bold text-gray-800 dark:text-gray-200 mb-3">Análise Intermediária</h4>
                    <p class="text-gray-600 dark:text-gray-300 text-sm mb-4">Perfeito para otimização mensal</p>
                    <div class="space-y-2 text-xs text-gray-700 dark:text-gray-300">
                      <p class="flex items-center justify-center">
                        <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                        </svg>
                        Tudo do Bronze +
                      </p>
                      <p class="flex items-center justify-center">
                        <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                        </svg>
                        Análise de categorias
                      </p>
                      <p class="flex items-center justify-center">
                        <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                        </svg>
                        3 recomendações personalizadas
                      </p>
                    </div>
                  </div>
                </div>

                <!-- Ouro -->
                <div class="group relative bg-gradient-to-br from-yellow-50 to-yellow-100 dark:from-yellow-900/20 dark:to-yellow-800/20 border-2 border-yellow-300 dark:border-yellow-600 rounded-2xl p-6 text-center transition-all duration-300 hover:scale-105 hover:shadow-lg cursor-pointer">
                  <div class="absolute -top-3 left-1/2 transform -translate-x-1/2">
                    <span class="bg-yellow-500 text-white px-4 py-1 rounded-full text-sm font-bold">OURO</span>
                  </div>
                  <div class="mt-4">
                    <h4 class="text-lg font-bold text-yellow-800 dark:text-yellow-200 mb-3">Análise Completa</h4>
                    <p class="text-yellow-600 dark:text-yellow-300 text-sm mb-4">Recomendado para planejamento estratégico</p>
                    <div class="space-y-2 text-xs text-yellow-700 dark:text-yellow-300">
                      <p class="flex items-center justify-center">
                        <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                        </svg>
                        Tudo do Prata +
                      </p>
                      <p class="flex items-center justify-center">
                        <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                        </svg>
                        Análise preditiva
                      </p>
                      <p class="flex items-center justify-center">
                        <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                        </svg>
                        Plano de ação personalizado
                      </p>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Penny Assistant -->
              
            </div>
          </div>
        </div>
      </section>

      <!-- Final CTA Section -->
      <section class="relative py-20">
        <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <div class="bg-gradient-to-br from-primary-500/10 to-primary-600/10 dark:from-primary-500/5 dark:to-primary-600/5 backdrop-blur-sm rounded-3xl p-12 border border-primary-200/50 dark:border-primary-700/50 shadow-2xl animate-pulse-slow">
            <h2 class="text-4xl md:text-5xl font-black text-gray-900 dark:text-white mb-6">
              Pronto para dar o <span class="text-transparent bg-clip-text bg-gradient-to-r from-primary-600 to-primary-400 dark:from-primary-400 dark:to-primary-300">primeiro passo</span>?
            </h2>
            
            <p class="text-xl text-gray-600 dark:text-gray-300 mb-8 max-w-2xl mx-auto leading-relaxed">
              Junte-se ao The Broke Club e descubra que controlar suas finanças pode ser mais simples do que você imagina.
            </p>

            <div class="flex flex-col sm:flex-row justify-center space-y-4 sm:space-y-0 sm:space-x-6">
              <router-link 
                to="/signup" 
                class="group relative bg-gradient-to-r from-primary-600 to-primary-500 hover:from-primary-700 hover:to-primary-600 text-white text-center py-4 px-10 text-lg font-semibold rounded-xl shadow-lg hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 hover:scale-105 overflow-hidden"
              >
                <span class="relative z-10 flex items-center justify-center">
                  Quero Me Organizar Agora
                  <svg class="w-5 h-5 ml-2 transform group-hover:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7l5 5-5 5M6 12h12"></path>
                  </svg>
                </span>
                <div class="absolute inset-0 bg-gradient-to-r from-white/20 to-transparent transform -skew-x-12 -translate-x-full group-hover:translate-x-full transition-transform duration-700"></div>
              </router-link>
            </div>

            <p class="mt-6 text-gray-500 dark:text-gray-400 text-sm">
              🚀 100% Gratuito • Setup em 2 minutos • Sem cartão de crédito
            </p>
          </div>
        </div>
      </section>

      <!-- Footer -->
      <footer class="relative py-12 border-t border-gray-200 dark:border-gray-800">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="text-center">
            <div class="flex justify-center items-center mb-6">
              <img src="../assets/logoBrokeDark2.png" alt="The Broke Club" class="h-8">
            </div>
            <p class="text-gray-500 dark:text-gray-400">
              Feito com 💚 para ajudar você a conquistar suas metas financeiras
            </p>
            <p class="text-gray-400 dark:text-gray-500 text-sm mt-2">
              The Broke Club © 2024 - Transformando vidas, um controle financeiro de cada vez
            </p>
          </div>
        </div>
      </footer>
    </div>
  </template>

  <script>
  export default {
    name: 'PremiumHomePage',
    mounted() {
      // Add intersection observer for scroll animations
      const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
          if (entry.isIntersecting) {
            entry.target.classList.add('animate-visible');
          }
        });
      }, { threshold: 0.1 });

      // Observe all animate-on-scroll elements
      document.querySelectorAll('.animate-fade-in-up').forEach(el => {
        observer.observe(el);
      });
    }
  }
  </script>

  <style scoped>
  /* Custom Animations */
  @keyframes float-slow {
    0%, 100% { transform: translateY(0px) rotate(0deg); }
    50% { transform: translateY(-20px) rotate(1deg); }
  }

  @keyframes float-medium {
    0%, 100% { transform: translateY(0px) rotate(0deg); }
    50% { transform: translateY(-15px) rotate(-1deg); }
  }

  @keyframes bounce-slow {
    0%, 100% { transform: translateY(0); }
    50% { transform: translateY(-10px); }
  }

  @keyframes typing {
    from { width: 0; }
    to { width: 100%; }
  }

  @keyframes gradient-x {
    0%, 100% { background-position: 0% 50%; }
    50% { background-position: 100% 50%; }
  }

  @keyframes slide-in-left {
    from { 
      opacity: 0;
      transform: translateX(-30px);
    }
    to { 
      opacity: 1;
      transform: translateX(0);
    }
  }

  @keyframes fade-in-up {
    from {
      opacity: 0;
      transform: translateY(30px);
    }
    to {
      opacity: 1;
      transform: translateY(0);
    }
  }

  @keyframes pulse-slow {
    0%, 100% { opacity: 1; }
    50% { opacity: 0.8; }
  }

  /* Animation Classes */
  .animate-float-slow {
    animation: float-slow 6s ease-in-out infinite;
  }

  .animate-float-medium {
    animation: float-medium 4s ease-in-out infinite;
  }

  .animate-bounce-slow {
    animation: bounce-slow 3s ease-in-out infinite;
  }

  .animate-typing {
    animation: typing 2s steps(20, end);
    white-space: nowrap;
    overflow: hidden;
  }

  .animate-gradient-x {
    background-size: 200% 200%;
    animation: gradient-x 3s ease infinite;
  }

  .animate-slide-in-left {
    animation: slide-in-left 0.8s ease-out;
  }

  .animate-fade-in-up {
    animation: fade-in-up 0.8s ease-out both;
  }

  .animate-pulse-slow {
    animation: pulse-slow 4s ease-in-out infinite;
  }

  /* Grid Pattern */
  .grid-pattern {
    background-image: 
      linear-gradient(rgba(99, 102, 241, 0.1) 1px, transparent 1px),
      linear-gradient(90deg, rgba(99, 102, 241, 0.1) 1px, transparent 1px);
    background-size: 50px 50px;
  }

  /* Scroll-triggered animations */
  .animate-fade-in-up {
    opacity: 0;
    transform: translateY(30px);
    transition: all 0.8s ease-out;
  }

  .animate-fade-in-up.animate-visible {
    opacity: 1;
    transform: translateY(0);
  }

  /* Backdrop blur support */
  @supports not (backdrop-filter: blur(10px)) {
    .backdrop-blur-sm {
      background-color: rgba(255, 255, 255, 0.9);
    }
    
    .dark .backdrop-blur-sm {
      background-color: rgba(17, 24, 39, 0.9);
    }
  }
  </style>