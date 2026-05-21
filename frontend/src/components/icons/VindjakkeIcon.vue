<script setup>
defineProps({
  size: { type: Number, default: 32 }
})
</script>

<template>
  <svg :width="size" :height="size" viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg">
    <path d="M22 12 Q22 4 32 4 Q42 4 42 12 L42 16 L22 16 Z" fill="#f7b733" stroke="#a86f10" stroke-width="2" stroke-linejoin="round"/>
    <path d="M20 12 L10 16 L8 38 L16 40 L18 30 L18 58 L46 58 L46 30 L48 40 L56 38 L54 16 L44 12 L42 16 L32 20 L22 16 Z"
          fill="#f7b733" stroke="#a86f10" stroke-width="2" stroke-linejoin="round"/>
    <line x1="32" y1="20" x2="32" y2="58" stroke="#a86f10" stroke-width="1.8"/>
    <line x1="32" y1="20" x2="32" y2="58" stroke="#ffffff" stroke-width="0.6" stroke-dasharray="2 1"/>
    <rect x="22" y="42" width="8" height="6" fill="none" stroke="#a86f10" stroke-width="1.3" rx="1"/>
    <rect x="34" y="42" width="8" height="6" fill="none" stroke="#a86f10" stroke-width="1.3" rx="1"/>
    <path d="M2 26 Q10 24 16 26 M0 36 Q8 34 12 36 M2 46 Q8 44 14 46" fill="none" stroke="#7fc4d8" stroke-width="2" stroke-linecap="round"/>
  </svg>
</template>
