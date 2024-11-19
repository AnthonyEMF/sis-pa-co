import { useState } from "react";
import { getLogsList } from "../../../shared/actions/logsdb/logsdb.action";

export const useLogs = () => {
    const [logs, setLogs] = useState({});
    const [isLoading, setIsLoading] = useState(false);

    // Cargar todas los registros
    const loadLogs = async (searchTerm, page) => {
        setIsLoading(true);

        const result = await getLogsList(searchTerm, page);
        setLogs(result);

        setIsLoading(false);
    }

    return {
        // Properties
        logs,
        isLoading,
        // Methods
        loadLogs,
    };
}