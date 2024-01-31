import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import Login from './pages/Login';
import Registration from './pages/Registration';
import Dashboard from './pages/Dashboard';
import axios from 'axios';
import Project from './pages/Project';

axios.interceptors.request.use((req) => {
  const token = localStorage.getItem('token');
  req.headers.Authorization = token ? `Bearer ${token}` : '';
  return req;
});

axios.interceptors.response.use(
  (res) => res,
  async (error) => {
    if (error.response.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
  },
);

const router = createBrowserRouter([
  {
    path: '/',
    element: <div>Hello world!</div>,
  },
  {
    path: '/login',
    element: <Login />,
  },
  {
    path: '/registration',
    element: <Registration />,
  },
  {
    path: '/dashboard',
    element: <Dashboard />,
  },
  {
    path: '/project/:id',
    element: <Project />,
  },
]);

ReactDOM.createRoot(document.getElementById('root')!).render(
  <RouterProvider router={router} />,
);
