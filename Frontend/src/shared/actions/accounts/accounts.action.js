import { sisPaCoApi } from '../../../config/api';

// Obtener paginación
export const getPaginationAccounts = async (searchTerm = "", page = 1) => {
    try {
        const {data} = await sisPaCoApi.get(`/accounts?searchTerm=${searchTerm}&page=${page}`);
        return data;
    } catch(error) {
        console.error(error);
        return error.response;
    }
}

// Obtener todo
export const getAllAccounts = async () => {
    try {
        const { data } = await sisPaCoApi.get(`/accounts/all`);
        return data;
    } catch (error) {
        console.error(error);
        return error.response;
    }
};

// Obtener por Id
export const getAccountById = async (id) => {
    try {
        const {data} = await sisPaCoApi.get(`/accounts/${id}`);
        return data;
    } catch(error) {
        console.error(error);
        return error.response;
    }
}

// Crear
export const createAccountApi = async (accountData) => {
    try {
        const {data} = await sisPaCoApi.post(`/accounts`, accountData);
        return data;
    } catch(error) {
        console.error(error);
        return error.response;
    }
}

// Editar 
export const editAccountApi = async (id, accountData) => {
    try {
        const {data} = await sisPaCoApi.put(`/accounts/${id}`, accountData);
        return data;
    } catch (error) {
        console.error(error);
        return error.response;
    }
}