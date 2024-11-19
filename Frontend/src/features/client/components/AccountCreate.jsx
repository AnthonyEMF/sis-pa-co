import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAccounts } from "../hooks/useAccounts";

export const AccountCreate = () => {
  const navigate = useNavigate();
  const [fetching, setFetching] = useState(true);
  const { accounts, loadAllAccounts, isLoading } = useAccounts();
  const { createAccount, isSubmitting, error } = useAccounts();
  const [accountData, setAccountData] = useState({
    parentId: null,
    name: "",
  });

  // Cargar cuentas
  useEffect(() => {
    if (fetching) {
      loadAllAccounts();
      setFetching(false);
    }
  }, [fetching]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setAccountData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      await createAccount(accountData);
      alert("Cuenta creada correctamente.");
      navigate("/catalogs");
    } catch {
      alert("Hubo un error al crear cuenta.");
    }
  };

  return (
    <div className="relative flex flex-col items-center w-full h-full p-8 bg-gray-100">
      <div className="flex flex-col w-full max-w-lg p-8 bg-white rounded-lg shadow-md">
        <h2 className="text-2xl font-bold text-gray-800 mb-4">
          Crear Nueva Cuenta
        </h2>
        {/* Formulario de ingreso */}
        <form onSubmit={handleSubmit}>
          <label htmlFor="name" className="mb-2 text-gray-700">
            Nombre de la Cuenta
          </label>
          <input
            id="name"
            name="name"
            type="text"
            value={accountData.name}
            onChange={handleChange}
            required
            placeholder="Ingrese el nombre de la cuenta"
            className="w-full px-4 py-2 mb-4 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
          />

          <label htmlFor="parentId" className="mb-2 text-gray-700">
            Cuenta Padre (opcional)
          </label>
          {isLoading ? (
            <li>Cargando cuentas...</li>
          ) : (
            <select
              id="parentId"
              name="parentId"
              value={accountData.parentId}
              onChange={handleChange}
              className="w-full px-4 py-2 mb-6 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">Selecciona una cuenta padre</option>
              {accounts?.data?.map((account) => (
                <option key={account.id} value={account.id}>
                  {account.name}
                </option>
              ))}
            </select>
          )}

          <button
            type="submit"
            disabled={isSubmitting}
            className={`w-full py-2 text-white font-bold rounded-lg shadow-lg ${
              accountData.name
                ? "bg-blue-500 hover:bg-blue-600"
                : "bg-gray-300 cursor-not-allowed"
            }`}
          >
            {isSubmitting ? "Creando..." : "Crear Cuenta"}
          </button>
          {error && (
            <div className="text-red-500 text-center mt-4">
              Ocurrió un error al crear la cuenta. Intenta nuevamente.
            </div>
          )}
        </form>
      </div>
    </div>
  );
};
