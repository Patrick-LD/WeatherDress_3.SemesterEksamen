<script setup>
defineProps({
  size: { type: Number, default: 32 }
})
</script>

<template>
  <svg :width="size" :height="size" viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg">
    <circle cx="20" cy="10" r="4" fill="#ffffff" stroke="#7a1f15" stroke-width="1.5"/>
    <path d="M6 28 Q6 14 20 12 Q34 14 34 28 L34 32 L6 32 Z"
          fill="#c0392b" stroke="#7a1f15" stroke-width="2" stroke-linejoin="round"/>
    <rect x="4" y="30" width="32" height="6" fill="#ffffff" stroke="#7a1f15" stroke-width="1.5"/>
    <path d="M14 32 L14 36 M20 32 L20 36 M26 32 L26 36" stroke="#7a1f15" stroke-width="1"/>
    <path d="M44 36 Q42 36 42 40 L42 56 Q42 60 46 60 L58 60 Q62 60 62 56 L62 46 L56 40 L54 42 L52 40 L50 42 L48 40 Z"
          fill="#c0392b" stroke="#7a1f15" stroke-width="1.8" stroke-linejoin="round"/>
    <path d="M56 40 L54 44 L56 48" fill="none" stroke="#7a1f15" stroke-width="1.5"/>
    <rect x="42" y="56" width="20" height="4" fill="#ffffff" stroke="#7a1f15" stroke-width="1.2"/>
  </svg>
</template>
