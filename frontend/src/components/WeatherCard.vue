<script setup>
import { capitalize } from '../utils/format.js'

defineProps({
  title: { type: String, required: true },
  data: { type: Object, required: true },
  sectionId: { type: String, required: true },
  isToday: { type: Boolean, default: false }
})
</script>

<template>
  <div class="card" :id="sectionId">
    <h2>{{ title }}</h2>
    <div class="big-temp" :id="isToday ? 'temperatur' : null">
      {{ Math.round(data.temperatureC) }}<span>°C</span>
    </div>
    <div class="badge" :id="isToday ? 'vejr-beskrivelse' : null">
      {{ capitalize(data.description) }}
    </div>
    <div class="weather-detail">
      <span class="label">Fugtighed</span>
      <strong :id="isToday ? 'fugtighed' : null">{{ data.humidity }} %</strong>
    </div>
    <div class="weather-detail">
      <span class="label">Vind</span>
      <strong>{{ data.windSpeed }} km/t</strong>
    </div>
    <div class="weather-detail">
      <span class="label">Nedbør</span>
      <strong>{{ data.precipitation }} mm</strong>
    </div>
  </div>
</template>
