import axios from 'axios';

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Интерцептор для обработки ошибок (опционально)
apiClient.interceptors.response.use(
  response => response,
  error => {
    // можно глобально показывать уведомления об ошибках
    return Promise.reject(error);
  }
);

export default apiClient;
