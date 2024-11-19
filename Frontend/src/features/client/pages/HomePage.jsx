export const HomePage = () => {
  return (
    <div className="flex flex-col items-center content-center w-full">
      <div className="max-w-5xl w-full p-6">
        <h1 className="text-3xl font-bold text-center text-gray-800 mb-8">Sis-Pa-Co</h1>
        <p className="text-xl text-center mb-2">Sistema de Partidas Contables</p>
        <p className="text-base text-center mb-0"><span className="font-semibold">Desarrollado por:</span> Anthony Miranda & Danilo Vides</p>
        <div className="grid grid-cols-1 mt-10 md:grid-cols-2 gap-6">
          {/* Catálogo de Cuentas */}
          <div className="bg-white shadow-lg rounded-lg p-6 hover:shadow-xl transition-shadow duration-200">
            <h2 className="text-xl font-semibold mb-2 text-gray-700">Catálogo de Cuentas</h2>
            <p className="text-gray-500 mb-6">Gestiona el catálogo de cuentas de tu sistema contable.</p>
          </div>

          {/* Partidas Contables */}
          <div className="bg-white shadow-lg rounded-lg p-6 hover:shadow-xl transition-shadow duration-200">
            <h2 className="text-xl font-semibold mb-2 text-gray-700">Partidas Contables</h2>
            <p className="text-gray-500 mb-6">Crea y gestiona las partidas contables fácilmente.</p>
          </div>

          {/* Saldos */}
          <div className="bg-white shadow-lg rounded-lg p-6 hover:shadow-xl transition-shadow duration-200">
            <h2 className="text-xl font-semibold mb-2 text-gray-700">Saldos de Cuentas</h2>
            <p className="text-gray-500 mb-6">Consulta los saldos actuales de tus cuentas.</p>
          </div>

          {/* Logs */}
          <div className="bg-white shadow-lg rounded-lg p-6 hover:shadow-xl transition-shadow duration-200">
            <h2 className="text-xl font-semibold mb-2 text-gray-700">Registro de Cambios</h2>
            <p className="text-gray-500 mb-6">Revisa el historial de cambios y acciones.</p>
          </div>
        </div>
      </div>
    </div>
  )
}
