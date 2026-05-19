<script setup>
import InputForm from './components/InputForm.vue'
import WeatherCard from './components/WeatherCard.vue'
import ClothingList from './components/ClothingList.vue'
import WeatherMessage from './components/WeatherMessage.vue'
import MotorButton from './components/MotorButton.vue'
import HourlyForecast from './components/HourlyForecast.vue'
import RecommendationHistory from './components/RecommendationHistory.vue'
import { useWeather } from './composables/useWeather.js'

const {
  postnummer,
  loading,
  fejl,
  visPanel,
  nuværende,
  gårMiddag,
  todayData,
  tøjListe,
  meddelelse,
  søgVejr,
  visForside
} = useWeather()
</script>

<template>
  <!-- Forside: postnummer-input -->
  <InputForm
    v-if="!visPanel"
    v-model="postnummer"
    :loading="loading"
    :fejl="fejl"
    @submit="søgVejr"
  />

  <!-- WeatherDress-panel -->
  <div v-else id="weatherdress-panel">
    <h1>WeatherDress</h1>
    <p class="panel-subtitle">{{ nuværende.location }} · {{ nuværende.date }}</p>

    <div class="weather-row">
      <div class="today-stack">
        <WeatherCard
          title="Vejr i dag"
          section-id="vejr-i-dag"
          :data="nuværende"
          :is-today="true"
        />
        <HourlyForecast :data="todayData" />
      </div>
      <WeatherCard
        title="Vejr i går"
        section-id="vejr-i-gaar"
        :data="gårMiddag"
      />
    </div>
    <ClothingList :items="tøjListe" />
    <WeatherMessage :text="meddelelse" />
    <MotorButton />
    <RecommendationHistory :zip-code="postnummer" />

    <span class="back-link" @click="visForside">← Søg nyt postnummer</span>
  </div>
</template>

<style>
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

#app { width: 90%; }

body {
  font-family: 'Segoe UI', sans-serif;
  background: linear-gradient(135deg, #74b9e7 0%, #d6eaf8 100%);
  min-height: 10vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px 0;
}

/* ── Forside ── */
#input-section {
  background: white;
  border-radius: 16px;
  padding: 48px 40px;
  box-shadow: 0 8px 32px rgba(0,0,0,0.12);
  text-align: center;
  width: 100%;
  max-width: 400px;
}

#input-section h1 {
  font-size: 2rem;
  color: #1a6da8;
  margin-bottom: 8px;
}

#input-section p {
  color: #666;
  margin-bottom: 28px;
  font-size: 0.95rem;
}

#postnummer-input {
  width: 100%;
  padding: 14px 18px;
  font-size: 1.1rem;
  border: 2px solid #cde4f5;
  border-radius: 10px;
  outline: none;
  text-align: center;
  letter-spacing: 4px;
  transition: border-color 0.2s;
}

#postnummer-input:focus { border-color: #1a6da8; }

#submit-button {
  margin-top: 16px;
  width: 100%;
  padding: 14px;
  font-size: 1rem;
  font-weight: 600;
  background: #1a6da8;
  color: white;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  transition: background 0.2s;
}

#submit-button:hover { background: #155d8f; }
#submit-button:disabled { background: #90b8d4; cursor: default; }

#error-message {
  margin-top: 16px;
  padding: 12px 16px;
  background: #fdecea;
  border: 1px solid #f5c2bb;
  border-radius: 8px;
  color: #c0392b;
  font-size: 0.9rem;
}

/* ── Resultat-panel ── */
#weatherdress-panel {
  width: 100%;
  padding: 0 16px;
}

#weatherdress-panel > h1 {
  font-size: 2rem;
  color: white;
  text-align: center;
  margin-bottom: 8px;
  text-shadow: 0 2px 8px rgba(0,0,0,0.15);
}

.panel-subtitle {
  text-align: center;
  color: rgba(255,255,255,0.85);
  margin-bottom: 24px;
  font-size: 0.95rem;
}

.weather-row {
  display: grid;
  grid-template-columns: minmax(380px, 1fr) minmax(380px, 1fr);
  gap: 16px;
  margin-bottom: 16px;
}

.card {
  background: white;
  border-radius: 14px;
  padding: 28px 40px;
  box-shadow: 0 4px 16px rgba(0,0,0,0.10);
  margin-bottom: 16px;
}

.card h2 {
  font-size: 0.85rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: #1a6da8;
  margin-bottom: 14px;
}

.weather-row .card { margin-bottom: 0; padding: 28px 64px; }

.today-stack {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.big-temp {
  font-size: 3rem;
  font-weight: 700;
  color: #1a1a2e;
  line-height: 1;
  margin-bottom: 8px;
}

.big-temp span {
  font-size: 1.5rem;
  font-weight: 400;
  color: #666;
}

.weather-detail {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-top: 10px;
  font-size: 0.9rem;
  color: #444;
}

.weather-detail .label {
  color: #888;
  min-width: 90px;
}

.badge {
  display: inline-block;
  margin-bottom: 10px;
  padding: 4px 12px;
  background: #eaf4fd;
  color: #1a6da8;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 500;
}

.clothing-list {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin-top: 8px;
}

.clothing-item {
  padding: 6px 14px;
  background: #eaf4fd;
  border: 1px solid #cde4f5;
  border-radius: 20px;
  font-size: 0.9rem;
  color: #1a6da8;
}

#meddelelse-om-vejr p {
  color: #444;
  line-height: 1.6;
  font-size: 0.95rem;
}

.back-link {
  display: block;
  text-align: center;
  margin-top: 4px;
  color: rgba(255,255,255,0.8);
  font-size: 0.85rem;
  cursor: pointer;
  text-decoration: underline;
}

.back-link:hover { color: white; }
</style>
