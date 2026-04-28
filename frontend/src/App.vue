<script setup>
import { ref, computed } from 'vue'

const postnummer = ref('')
const loading = ref(false)
const fejl = ref('')
const visPanel = ref(false)
const todayData = ref([])
const yesterdayData = ref([])

const nuværende = computed(() => findNuværendeTime(todayData.value))
const gårMiddag = computed(() => findMiddag(yesterdayData.value))
const tøjListe = computed(() => beregnTøj(nuværende.value))
const meddelelse = computed(() => lavMeddelelse(nuværende.value))

async function søgVejr() {
  const zip = postnummer.value.trim()
  if (!zip) return

  loading.value = true
  fejl.value = ''

  try {
    const [today, yesterday] = await Promise.all([
      hentData(`/api/weatherforecast/${zip}/today`),
      hentData(`/api/weatherforecast/${zip}/yesterday`)
    ])
    todayData.value = today
    yesterdayData.value = yesterday
    visPanel.value = true
  } catch (err) {
    fejl.value = err.message
  } finally {
    loading.value = false
  }
}

async function hentData(url) {
  const res = await fetch(url)
  if (!res.ok) {
    const tekst = await res.text()
    throw new Error(tekst || `Fejl fra server (${res.status})`)
  }
  return res.json()
}

function visForside() {
  visPanel.value = false
  postnummer.value = ''
  fejl.value = ''
}

function findNuværendeTime(data) {
  if (!data.length) return {}
  const targetTime = new Date().getHours().toString().padStart(2, '0') + ':00'
  return data.find(e => e.time === targetTime) ?? data[0]
}

function findMiddag(data) {
  if (!data.length) return {}
  return data.find(e => e.time === '12:00') ?? data[12] ?? data[0]
}

function beregnTøj(e) {
  if (!e || e.temperatureC === undefined) return []
  const { temperatureC: temp, windSpeed: vind, precipitation: nedbor } = e
  const items = []

  if (temp < 0)       items.push('Tyk vinterjakke', 'Hue og handsker', 'Vinterstøvler')
  else if (temp < 5)  items.push('Varm jakke', 'Hue og handsker')
  else if (temp < 10) items.push('Jakke', 'Sweater')
  else if (temp < 15) items.push('Let jakke')
  else if (temp < 20) items.push('Langærmet trøje')
  else                items.push('T-shirt')

  if (nedbor > 0) items.push('Regnjakke / paraply')
  if (vind > 10)  items.push('Vindjakke')

  return items
}

function lavMeddelelse(e) {
  if (!e || !e.location) return ''
  let msg = `Det er ${capitalize(e.description)} i ${e.location} med ${Math.round(e.temperatureC)}°C. `

  if (e.windSpeed > 15)     msg += 'Der er kraftig vind — hold godt fast i hatten. '
  else if (e.windSpeed > 8) msg += 'Der er en del vind. '

  if (e.precipitation > 5)      msg += 'Det regner kraftigt — tag din paraply med!'
  else if (e.precipitation > 0) msg += 'Der er let regn.'
  else                          msg += 'Nyd din dag!'

  return msg
}

function capitalize(str) {
  if (!str) return ''
  return str.charAt(0).toUpperCase() + str.slice(1)
}
</script>

<template>
  <!-- Forside: postnummer-input -->
  <div v-if="!visPanel" id="input-section">
    <h1>WeatherDress</h1>
    <p>Indtast dit postnummer for at få vejr og påklædningsforslag</p>
    <input
      id="postnummer-input"
      v-model="postnummer"
      @keydown.enter="søgVejr"
      type="text"
      placeholder="Postnummer"
      maxlength="4"
      inputmode="numeric"
    />
    <button id="submit-button" @click="søgVejr" :disabled="loading">
      {{ loading ? 'Henter vejr…' : 'Søg vejr' }}
    </button>
    <div v-if="fejl" id="error-message">{{ fejl }}</div>
  </div>

  <!-- WeatherDress-panel -->
  <div v-else id="weatherdress-panel">
    <h1>WeatherDress</h1>
    <p class="panel-subtitle">{{ nuværende.location }} · {{ nuværende.date }}</p>

    <div class="weather-row">
      <!-- Vejr i dag -->
      <div class="card" id="vejr-i-dag">
        <h2>Vejr i dag</h2>
        <div class="big-temp" id="temperatur">{{ Math.round(nuværende.temperatureC) }}<span>°C</span></div>
        <div class="badge" id="vejr-beskrivelse">{{ capitalize(nuværende.description) }}</div>
        <div class="weather-detail">
          <span class="label">Fugtighed</span>
          <strong id="fugtighed">{{ nuværende.humidity }} %</strong>
        </div>
        <div class="weather-detail">
          <span class="label">Vind</span>
          <strong>{{ nuværende.windSpeed }} km/t</strong>
        </div>
        <div class="weather-detail">
          <span class="label">Nedbør</span>
          <strong>{{ nuværende.precipitation }} mm</strong>
        </div>
      </div>

      <!-- Vejr i går -->
      <div class="card" id="vejr-i-gaar">
        <h2>Vejr i går</h2>
        <div class="big-temp">{{ Math.round(gårMiddag.temperatureC) }}<span>°C</span></div>
        <div class="badge">{{ capitalize(gårMiddag.description) }}</div>
        <div class="weather-detail">
          <span class="label">Fugtighed</span>
          <strong>{{ gårMiddag.humidity }} %</strong>
        </div>
        <div class="weather-detail">
          <span class="label">Vind</span>
          <strong>{{ gårMiddag.windSpeed }} km/t</strong>
        </div>
        <div class="weather-detail">
          <span class="label">Nedbør</span>
          <strong>{{ gårMiddag.precipitation }} mm</strong>
        </div>
      </div>
    </div>

    <!-- Tøj anbefalet -->
    <div class="card" id="toj-anbefalet">
      <h2>Tøj anbefalet</h2>
      <div class="clothing-list">
        <span v-for="item in tøjListe" :key="item" class="clothing-item">{{ item }}</span>
      </div>
    </div>

    <!-- Meddelelse om vejr -->
    <div class="card" id="meddelelse-om-vejr">
      <h2>Meddelelse omkring vejr</h2>
      <p>{{ meddelelse }}</p>
    </div>

    <span class="back-link" @click="visForside">← Søg nyt postnummer</span>
  </div>
</template>

<style>
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

body {
  font-family: 'Segoe UI', sans-serif;
  background: linear-gradient(135deg, #74b9e7 0%, #d6eaf8 100%);
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px;
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
  max-width: 800px;
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
  grid-template-columns: 1fr 1fr;
  gap: 16px;
  margin-bottom: 16px;
}

.card {
  background: white;
  border-radius: 14px;
  padding: 20px 24px;
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

.weather-row .card { margin-bottom: 0; }

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
