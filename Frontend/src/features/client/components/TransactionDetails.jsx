import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useTransactions } from "../hooks/useTransactions";
import { formatDate } from "../../../shared/utils/format-date";

export const TransactionDetails = () => {
    const { id } = useParams();
    const { transaction, loadTransactionById, editTransaction } = useTransactions();
    const [fetching, setFetching] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
      if (fetching) {
        if (id) {
          loadTransactionById(id);
          setFetching(false);
        }
      }
    }, [fetching]);

    if (!transaction || !transaction.data) {
      return <div>Partida no encontrada.</div>;
    }

    // Editar el estado de la partida
    const handleEditTransaction = async () => {
      try {
        // Cambiar el estado de la partida a inactiva
        await editTransaction(id, { isActive: false });
        alert("Partida dada de baja exitosamente, se reajustaron los saldos.");
        navigate('/transactions');
      } catch (error) {
        console.error("Error al dar de baja la partida:", error);
        alert("Ocurrió un error al intentar dar de baja la partida.");
      }
    };

    return (
      <div className="max-w-4xl mb-8 mt-8 mx-auto p-6 bg-white shadow-md rounded-md">
      <h1 className="text-2xl font-semibold mb-4">Detalles de la Partida Contable</h1>

      <div className="flex flex-col space-y-4 mb-6">
        <div className="w-full">
          <label className="block text-sm font-medium text-gray-600">Fecha:</label>
          <input
            type="text"
            value={formatDate(transaction.data.date)}
            readOnly
            className="mt-1 p-2 w-full bg-gray-100 border border-gray-300 rounded-md"
          />
        </div>

        <div className="w-full">
          <label className="block text-sm font-medium text-gray-600">Descripción:</label>
          <input
            type="text"
            value={transaction.data.description}
            readOnly
            className="mt-1 p-2 w-full bg-gray-100 border border-gray-300 rounded-md"
          />
        </div>
      </div>

      <h2 className="text-xl font-semibold mt-6 mb-4">Entradas</h2>
      <div className="space-y-4">
        {transaction.data.entries.map((entry) => (
          <div key={entry.id} className="flex space-x-4">
            <div className="w-1/3">
              <label className="block text-sm font-medium text-gray-600">Cuenta:</label>
              <input
                type="text"
                value={entry.accountName}
                readOnly
                className="mt-1 p-2 w-full bg-gray-100 border border-gray-300 rounded-md"
              />
            </div>

            <div className="w-1/3">
              <label className="block text-sm font-medium text-gray-600">Tipo:</label>
              <input
                type="text"
                value={entry.type}
                readOnly
                className="mt-1 p-2 w-full bg-gray-100 border border-gray-300 rounded-md"
              />
            </div>

            <div className="w-1/3">
              <label className="block text-sm font-medium text-gray-600">Monto:</label>
              <input
                type="text"
                value={entry.amount}
                readOnly
                className="mt-1 p-2 w-full bg-gray-100 border border-gray-300 rounded-md"
              />
            </div>
          </div>
        ))}
      </div>

      <div className="mt-6 flex justify-between">
        <div>
          <span className="text-sm font-medium">Total Débito:</span>
          <span className="font-semibold text-blue-700"> ${transaction.data.totalDebit}</span>
        </div>
        <div>
          <span className="text-sm font-medium">Total Crédito:</span>
          <span className="font-semibold text-blue-700"> ${transaction.data.totalCredit}</span>
        </div>
      </div>

      <div className="flex mt-6 justify-center">
        {transaction.data.isActive && (
          <button
            onClick={handleEditTransaction}
            className="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700"
          >
            Dar de Baja la Partida
          </button>
        )}
      </div>
    </div>
  )
}