import { useEffect, useState } from "react";
import { useTransactions } from "../hooks/useTransactions";
import { useAccounts } from "../hooks/useAccounts";
import { jwtDecode } from "jwt-decode";
import { useNavigate } from "react-router-dom";

export const TransactionCreate = () => {
  const { accounts, loadAllAccounts } = useAccounts();
  const { createTransaction, isSubmitting, error } = useTransactions();
  const [fetching, setFetching] = useState(true);
  const [description, setDescription] = useState("");
  const [entries, setEntries] = useState([]);
  const [totalCredit, setTotalCredit] = useState(0);
  const [totalDebit, setTotalDebit] = useState(0);
  const navigate = useNavigate();

  // Decodificar el token para obtener el UserId (npm install jwt-decode)
  const userId = jwtDecode(localStorage.getItem("token")).UserId;

  // Cargar cuentas
  useEffect(() => {
    if (fetching) {
      loadAllAccounts();
      setFetching(false);
    }
  }, [fetching]);

  const addEntry = () => {
    setEntries([...entries, { accountId: "", type: "DÉBITO", amount: 0 }]);
  };

  const removeEntry = (index) => {
    const updatedEntries = entries.filter((_, i) => i !== index);
    setEntries(updatedEntries);
    calculateTotals(updatedEntries);
  };

  const handleEntryChange = (index, field, value) => {
    const updatedEntries = entries.map((entry, i) =>
      i === index ? { ...entry, [field]: value } : entry
    );
    setEntries(updatedEntries);
    calculateTotals(updatedEntries);
  };

  const calculateTotals = (updatedEntries) => {
    const credit = updatedEntries
      .filter((entry) => entry.type === "CRÉDITO")
      .reduce((acc, entry) => acc + parseFloat(entry.amount || 0), 0);
    const debit = updatedEntries
      .filter((entry) => entry.type === "DÉBITO")
      .reduce((acc, entry) => acc + parseFloat(entry.amount || 0), 0);
    setTotalCredit(credit);
    setTotalDebit(debit);
  };

  const handleSubmit = () => {
    // Validaciones
    if (!description) {
      alert("Debe ingresar una descripción.");
      return;
    }
    if (entries.length === 0) {
      alert("Debe ingresar entradas.");
      return;
    }
    for (const entry of entries) {
      if (entry.amount <= 0) {
        alert("El monto de las entradas debe ser mayor que cero.");
        return;
      }
    }
    if (totalDebit !== totalCredit) {
      alert("El total de débitos debe debe cuadrar con el total de créditos.");
      return;
    }

    const transactionData = {
      userId,
      description,
      entries: entries.map((entry) => ({
        accountId: entry.accountId,
        amount: entry.amount,
        type: entry.type,
      })),
    };

    createTransaction(transactionData);
    alert("Partida registrada con éxito, se han actualizado los saldos.");
    navigate("/balances");
  };

  return (
    <div className="p-4 bg-white rounded shadow-md max-w-3xl mx-auto my-6">
      <h1 className="text-2xl font-bold mb-4">Crear Partida Contable</h1>

      <div className="mb-4">
        <label className="block text-gray-700">Fecha:</label>
        <input
          type="text"
          className="w-full bg-gray-200 mt-1 p-2 border rounded"
          value={new Date().toLocaleDateString()}
          readOnly
        />
      </div>

      <div className="mb-4">
        <label className="block text-gray-700">Descripción:</label>
        <input
          type="text"
          className="w-full mt-1 p-2 border rounded"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          placeholder="Descripción de la partida"
          required
        />
      </div>

      <div className="mb-4">
        <h2 className="text-lg font-bold">Entradas</h2>
        {entries.map((entry, index) => (
          <div key={index} className="flex items-center space-x-2 mt-2">
            <select
              className="w-1/3 p-2 border rounded"
              value={entry.accountId}
              onChange={(e) =>
                handleEntryChange(index, "accountId", e.target.value)
              }
            >
              <option value="">Seleccione cuenta</option> {/* Filtrar cuentas con allowMovement = true */}
              {accounts?.data?.filter((account) => account.allowMovement)
                .map((account) => (
                  <option key={account.id} value={account.id}>
                    {account.name}
                  </option>
                ))}
            </select>

            <select
              className="w-1/4 p-2 border rounded"
              value={entry.type}
              onChange={(e) => handleEntryChange(index, "type", e.target.value)}
            >
              <option value="DÉBITO">Débito</option>
              <option value="CRÉDITO">Crédito</option>
            </select>

            <input
              type="number"
              className="w-1/4 p-2 border rounded"
              value={entry.amount}
              onChange={(e) =>
                handleEntryChange(index, "amount", e.target.value)
              }
              placeholder="Monto"
            />

            <button
              onClick={() => removeEntry(index)}
              className="bg-red-500 text-white p-2 rounded"
            >
              Borrar
            </button>
          </div>
        ))}
        <button
          onClick={addEntry}
          className="mt-4 bg-blue-500 text-white px-4 py-2 rounded"
        >
          Agregar Entrada
        </button>
      </div>

      <div className="mt-4">
        {entries.length > 0 && (
          <div className="flex justify-between font-bold">
            <span>
              Total Débito:{" "}
              <span className="text-blue-700">${totalDebit.toFixed(2)}</span>
            </span>
            <span>
              Total Crédito:{" "}
              <span className="text-blue-700">${totalCredit.toFixed(2)}</span>
            </span>
          </div>
        )}
      </div>

      {error && <div className="text-red-500 mt-4">{error.message}</div>}

      <button
        onClick={handleSubmit}
        className="mt-4 bg-green-600 text-white px-4 py-2 rounded w-full"
        disabled={isSubmitting}
      >
        {isSubmitting ? "Registrando..." : "Registrar Partida"}
      </button>
    </div>
  );
};
