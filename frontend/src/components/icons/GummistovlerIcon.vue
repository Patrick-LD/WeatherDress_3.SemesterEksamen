<script setup>
defineProps({
  size: { type: Number, default: 32 }
})
</script>

<template>
  <svg :width="size" :height="size" viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg">
    <path d="M22 6 L40 6 Q42 6 42 8 L42 42 L52 42 Q58 42 58 48 L58 54 Q58 58 54 58 L16 58 Q12 58 12 54 L12 48 Q12 42 18 42 L22 42 Z"
          fill="#e8c020" stroke="#b08000" stroke-width="2" stroke-linejoin="round"/>
    <rect x="12" y="52" width="46" height="6" fill="#1a1a1a" stroke="#1a1a1a" stroke-width="1"/>
    <path d="M44 46 L54 46 M44 50 L54 50" stroke="#b08000" stroke-width="1" opacity="0.5"/>
  </svg>
</template>
