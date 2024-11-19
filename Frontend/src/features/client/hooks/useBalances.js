import { useState } from "react";
import { getBalancesList } from "../../../shared/actions/balances/balances.action";

export const useBalances = () => {
    const [balances, setBalances] = useState({});
    const [isLoading, setIsLoading] = useState(false);

    // Cargar todas los saldos
    const loadBalances = async (searchTerm, page) => {
        setIsLoading(true);

        const result = await getBalancesList(searchTerm, page);
        setBalances(result);

        setIsLoading(false);
    }

    return {
        // Properties
        balances,
        isLoading,
        // Methods
        loadBalances,
    };
}