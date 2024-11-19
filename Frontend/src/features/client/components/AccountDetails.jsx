import { useEffect, useState } from "react";
import { useAccounts } from "../hooks/useAccounts";
import { useNavigate, useParams } from "react-router-dom";

export const AccountDetails = () => {
  const { id } = useParams();
  const { account, loadAccountById, editAccount } = useAccounts();
  const [fetching, setFetching] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    if (fetching && id) {
      loadAccountById(id).finally(() => setFetching(false));
    }
  }, [fetching, id, loadAccountById]);

  if (!account || !account.data) {
    return <div>Cuenta no encontrada.</div>;
  }

  // Cambiar el estado de la cuenta
  const ToggleEditAccount = async () => {
    try {
      const updatedState = !account.data.isActive; 
      await editAccount(id, { isActive: updatedState });
      alert(`La cuenta ahora está ${updatedState ? "ACTIVA" : "INACTIVA"}.`);
      navigate('/catalogs'); 
    } catch (error) {
      console.error("Error al cambiar el estado de la cuenta:", error);
      alert("Ocurrió un error al intentar cambiar el estado de la cuenta.");
    }
  };

  const shouldShowButton =
    (account.data.allowMovement && account.data.isActive) ||
    (!account.data.allowMovement && !account.data.isActive);

  return (
    <div className="max-w-4xl mx-auto my-6 p-6 bg-white shadow-md rounded-md">
      <h1 className="text-2xl font-semibold mb-4">Detalles de la Cuenta</h1>

      <div className="flex flex-col space-y-4 mb-6">
        <div className="w-full">
          <label className="block text-sm font-medium text-gray-600">Código</label>
          <input
            type="text"
            value={account.data.code}
            readOnly
            className="mt-1 p-2 w-full bg-gray-100 border border-gray-300 rounded-md"
          />
        </div>

        <div className="w-full">
          <label className="block text-sm font-medium text-gray-600">Nombre</label>
          <input
            type="text"
            value={account.data.name}
            readOnly
            className="mt-1 p-2 w-full bg-gray-100 border border-gray-300 rounded-md"
          />
        </div>

        <div className="w-full mt-4">
          <label className="block text-sm font-medium text-gray-600">Permite movimiento:</label>
          <div
            className={`text-center mt-1 p-2 w-full rounded-md ${
              account.data.allowMovement ? "bg-green-100 text-green-600" : "bg-red-100 text-red-600"
            }`}
          >
            {account.data.allowMovement ? "SI" : "NO"}
          </div>
        </div>

        <div className="w-full mt-4">
          <label className="block text-sm font-medium text-gray-600">Estado de la cuenta</label>
          <div
            className={`text-center mt-1 p-2 w-full rounded-md ${
              account.data.isActive ? "bg-green-100 text-green-600" : "bg-red-100 text-red-600"
            }`}
          >
            {account.data.isActive ? "ACTIVA" : "INACTIVA"}
          </div>
        </div>

        {shouldShowButton && (
          <div className="flex mt-6 justify-center">
            <button
              onClick={ToggleEditAccount}
              className={`px-4 py-2 rounded-md text-white ${
                account.data.isActive
                  ? "bg-red-600 hover:bg-red-700"
                  : "bg-green-600 hover:bg-green-700"
              }`}
            >
              {account.data.isActive ? "Desactivar Cuenta" : "Activar Cuenta"}
            </button>
          </div>
        )}
      </div>
    </div>
  );
};
