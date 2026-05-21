<script setup>
import { computed } from 'vue'
import { capitalize } from '../utils/format.js'
import SolIcon from './icons/SolIcon.vue'
import RegnIcon from './icons/RegnIcon.vue'
import SkyetIcon from './icons/SkyetIcon.vue'
import SneIcon from './icons/SneIcon.vue'
import TshirtIcon from './icons/TshirtIcon.vue'
import LangaermetIcon from './icons/LangaermetIcon.vue'
import LetJakkeIcon from './icons/LetJakkeIcon.vue'
import JakkeIcon from './icons/JakkeIcon.vue'
import SweaterIcon from './icons/SweaterIcon.vue'
import VarmJakkeIcon from './icons/VarmJakkeIcon.vue'
import VinterjakkeIcon from './icons/VinterjakkeIcon.vue'
import HueHandskerIcon from './icons/HueHandskerIcon.vue'
import StovlerIcon from './icons/StovlerIcon.vue'
import ParaplyIcon from './icons/ParaplyIcon.vue'
import VindjakkeIcon from './icons/VindjakkeIcon.vue'

const clothingIcons = {
  'T-shirt': TshirtIcon,
  'Langærmet trøje': LangaermetIcon,
  'Let jakke': LetJakkeIcon,
  'Jakke': JakkeIcon,
  'Sweater': SweaterIcon,
  'Varm jakke': VarmJakkeIcon,
  'Tyk vinterjakke': VinterjakkeIcon,
  'Hue og handsker': HueHandskerIcon,
  'Vinterstøvler': StovlerIcon,
  'Regnjakke / paraply': ParaplyIcon,
  'Vindjakke': VindjakkeIcon
}

const props = defineProps({
  title: { type: String, required: true },
  data: { type: Object, required: true },
  sectionId: { type: String, required: true },
  isToday: { type: Boolean, default: false },
  iconSize: { type: Number, default: 64 },
  items: { type: Array, default: () => [] }
})

const weatherIcon = computed(() => {
  const desc = props.data.description?.toLowerCase() ?? ''
  if (desc.includes('sne')) return SneIcon
  if (desc.includes('regn') || desc.includes('støvregn') || desc.includes('torden')) return RegnIcon
  if (desc.includes('skyet') || desc.includes('overskyet') || desc.includes('tåge')) return SkyetIcon
  if (desc.includes('klar') || desc.includes('sol')) return SolIcon
  return null
})
</script>

<template>
  <div class="card" :id="sectionId">
    <h2>{{ title }}</h2>
    <div :class="items.length ? 'card-inner' : ''">
      <div class="weather-info">
        <component :is="weatherIcon" v-if="weatherIcon" :size="iconSize" class="weather-icon" />
        <div class="big-temp" :id="isToday ? 'temperatur' : 'temperatur-igaar'">
          {{ Math.round(data.temperatureC) }}<span>°C</span>
        </div>
        <div class="badge" :id="isToday ? 'vejr-beskrivelse' : 'vejr-beskrivelse-igaar'">
          {{ capitalize(data.description) }}
        </div>
        <div class="weather-detail">
          <span class="label">Fugtighed</span>
          <strong :id="isToday ? 'fugtighed' : 'fugtighed-igaar'">{{ data.humidity }} %</strong>
        </div>
        <div class="weather-detail">
          <span class="label">Vind</span>
          <strong :id="isToday ? 'vindstyrke' : 'vindstyrke-igaar'">{{ data.windSpeed }} km/t</strong>
        </div>
        <div class="weather-detail">
          <span class="label">Nedbør</span>
          <strong :id="isToday ? 'nedbor' : 'nedbor-igaar'">{{ data.precipitation }} mm</strong>
        </div>
      </div>

      <div v-if="items.length" class="clothing-panel">
        <p class="clothing-title">Hvad skal du have på?</p>
        <ul class="clothing-panel-list">
          <li v-for="item in items" :key="item" class="clothing-panel-item">
            <component :is="clothingIcons[item]" v-if="clothingIcons[item]" :size="40" class="clothing-icon" />
            <span>{{ item }}</span>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<style scoped>
.card-inner {
  display: flex;
  gap: 24px;
  align-items: flex-start;
}

.weather-info {
  flex: 1;
}

.clothing-panel {
  flex: 1;
  border-left: 2px solid #eaf4fd;
  padding-left: 24px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  min-height: 100%;
}

.clothing-title {
  font-size: 0.85rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: #1a6da8;
  margin-bottom: 16px;
}

.clothing-panel-list {
  list-style: none;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.clothing-panel-item {
  font-size: 1rem;
  color: #1a1a2e;
  padding: 8px 16px;
  background: #eaf4fd;
  border-radius: 10px;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 12px;
}

.clothing-icon {
  flex-shrink: 0;
}
</style>
