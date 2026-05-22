<script setup>
import InputForm from './components/InputForm.vue'
import WeatherCard from './components/WeatherCard.vue'
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

    <div class="weather-grid">
      <WeatherCard
        title="Vejr i dag"
        section-id="vejr-i-dag"
        :data="nuværende"
        :is-today="true"
        :items="tøjListe"
      />
      <WeatherCard
        title="Vejr i går"
        section-id="vejr-i-gaar"
        :data="gårMiddag"
        :icon-size="48"
        class="yesterday-card"
      />
      <div class="forecast-message-row">
        <HourlyForecast :data="todayData" />
        <WeatherMessage :text="meddelelse" />
      </div>
      <MotorButton :zip-code="postnummer" />
    </div>
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

.weather-grid {
  display: grid;
  grid-template-columns: minmax(380px, 2fr) minmax(220px, 1fr);
  grid-template-areas:
    "today    yesterday"
    "forecast motor";
  gap: 16px;
  margin-bottom: 16px;
}

#vejr-i-dag           { grid-area: today; }
#vejr-i-gaar          { grid-area: yesterday; }
.forecast-message-row { grid-area: forecast; }
#motor-styring        { grid-area: motor; }

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

.weather-grid .card { margin-bottom: 0; padding: 28px 40px; }

.forecast-message-row {
  display: flex;
  gap: 16px;
  align-items: stretch;
}

.forecast-message-row > * {
  flex: 1;
  min-width: 0;
}

.forecast-message-row #hourly-forecast {
  flex: 1.8;
}

.forecast-message-row #meddelelse-om-vejr {
  flex: 1;
}

.yesterday-card.card {
  padding: 20px 24px;
}

.yesterday-card .big-temp {
  font-size: 2.2rem;
}

.yesterday-card .weather-icon {
  width: 48px;
  height: 48px;
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
