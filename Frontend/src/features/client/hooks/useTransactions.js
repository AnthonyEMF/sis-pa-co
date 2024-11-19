import { useState } from "react";
import { createTransactionApi, editTransactionApi, getTransactionById, getTransactionsList } from "../../../shared/actions/transactions/transactions.action";

export const useTransactions = () => {
    const [transactions, setTransactions] = useState({});
    const [transaction, setTransaction] = useState({});
    const [isLoading, setIsLoading] = useState(false);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState(null);

    // Cargar todas las partidas
    const loadTransactions = async (searchTerm, page) => {
        setIsLoading(true);

        const result = await getTransactionsList(searchTerm, page);
        setTransactions(result);

        setIsLoading(false);
    }

    // Cargar partida por Id
    const loadTransactionById = async (id) => {
        const result = await getTransactionById(id);
        setTransaction(result);
    }

    // Crear partida
    const createTransaction = async (transactionData) => {
        setIsSubmitting(true);
        setError(null);

        try {
            const result = await createTransactionApi(transactionData);
            setTransaction(result);
        } catch(error) {
            setError(error)
        } finally {
            setIsSubmitting(false);
        }
    }

    // Editar partida
    const editTransaction = async (id, transactionData) => {
        setIsSubmitting(true);
        setError(null);

        try {
            const result = await editTransactionApi(id, transactionData);
            setTransaction(result);
        } catch (error) {
            setError(error);
        } finally {
            setIsSubmitting(false);
        }
    };

    return {
        // Properties
        transactions,
        transaction,
        isLoading,
        isSubmitting,
        error,
        // Methods
        loadTransactions,
        loadTransactionById,
        createTransaction,
        editTransaction,
    };
}