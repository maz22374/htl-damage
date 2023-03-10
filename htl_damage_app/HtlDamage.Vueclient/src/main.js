import { createApp } from 'vue'
import axios from "axios";             // npm install axios
import process from 'node:process'
import App from './App.vue'
import router from './router'
import store from './store.js'
import 'bootstrap/dist/css/bootstrap.css'
import './assets/main.css'

axios.defaults.baseURL = process.env.NODE_ENV == 'production' ? "/api" : "https://localhost:5001/api";

const app = createApp(App)
app.use(router)
app.use(store)
app.mount('#app')
