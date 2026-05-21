<script setup>
defineProps({
  size: { type: Number, default: 32 }
})
</script>

<template>
  <svg :width="size" :height="size" viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg">
    <path d="M22 14 Q22 6 32 6 Q42 6 42 14 L42 18 L22 18 Z" fill="#3a2a4a" stroke="#1a0f2d" stroke-width="2" stroke-linejoin="round"/>
    <path d="M18 12 L6 18 L4 40 L14 42 L16 32 L16 60 L48 60 L48 32 L50 42 L60 40 L58 18 L46 12 L42 18 L32 22 L22 18 Z"
          fill="#5a3a7a" stroke="#1a0f2d" stroke-width="2" stroke-linejoin="round"/>
    <line x1="32" y1="22" x2="32" y2="60" stroke="#1a0f2d" stroke-width="2"/>
    <line x1="32" y1="22" x2="32" y2="60" stroke="#ffd93d" stroke-width="0.8" stroke-dasharray="2 1"/>
    <rect x="20" y="44" width="9" height="7" fill="none" stroke="#1a0f2d" stroke-width="1.5" rx="1"/>
    <rect x="35" y="44" width="9" height="7" fill="none" stroke="#1a0f2d" stroke-width="1.5" rx="1"/>
  </svg>
</template>
