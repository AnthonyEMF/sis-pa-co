import { sisPaCoApi } from '../../../config/api';

// Obtener todos
export const getTransactionsList = async (searchTerm = "", page = 1) => {
    try {
        const {data} = await sisPaCoApi.get(`/transactions?searchTerm=${searchTerm}&page=${page}`);
        return data;
    } catch(error) {
        console.error(error);
        return error.response;
    }
}

// Obtener por Id
export const getTransactionById = async (id) => {
    try {
        const {data} = await sisPaCoApi.get(`/transactions/${id}`);
        return data;
    } catch(error) {
        console.error(error);
        return error.response;
    }
}

// Crear
export const createTransactionApi = async (transactionData) => {
    try {
        const {data} = await sisPaCoApi.post(`/transactions`, transactionData);
        return data;
    } catch(error) {
        console.error(error);
        return error.response;
    }
}

// Editar 
export const editTransactionApi = async (id, transactionData) => {
    try {
        const {data} = await sisPaCoApi.put(`/transactions/${id}`, transactionData);
        return data;
    } catch (error) {
        console.error(error);
        return error.response;
    }
}