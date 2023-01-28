import { createRouter, createWebHistory } from 'vue-router'
import store from '../store.js'
import Login from '../views/Login.vue'
import HomeView from '../views/HomeView.vue'
import NewDamage from '../views/NewDamage.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/newdamage',
      name: 'newdamage',
      //meta: { authorize: true },
      component: NewDamage
    },
    {
      path: '/login',
      name: 'login',
      component: Login
    }
  ]
});

router.beforeEach((to, from, next) => {
  const authenticated = store.state.userdata.username ? true : false;
  if (to.meta.authorize && !authenticated) {
    next("/login");
    return;
  }
  next();
  return;
});

export default router;