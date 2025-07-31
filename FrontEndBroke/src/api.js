import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5024",
});

export default api;
