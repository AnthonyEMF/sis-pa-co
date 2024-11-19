import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom"
import { useAuthStore } from "../store";
import { useFormik } from "formik";
import { Loading } from "../../../shared/components/Loading";
import { loginInitValues, loginValidationSchema } from "../forms/login.data";

export const LoginPage = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);
  const login = useAuthStore( (state) => state.login );
  const error = useAuthStore( (state) => state.error );
  const message = useAuthStore( (state) => state.message );

  useEffect(() => {
    if (isAuthenticated) {
      navigate('/home');
    }
  }, [isAuthenticated])

  const formik = useFormik({
    initialValues: loginInitValues ,
    validationSchema: loginValidationSchema,
    validateOnChange: true,
    onSubmit: async (formValues) => {
      //console.log(formValues);
      setLoading(true);
      await login(formValues);
      setLoading(false);
    }
  });

  if (loading) {
    return <Loading/>
  }

  return (
    <div className="p-10 xs:p-0 mx-auto md:w-full md:max-w-md mb-5">
      <div className="p-8 xs:p-4 w-full max-w-md bg-white rounded-lg shadow-md">
        <h1 className="text-2xl font-bold text-center text-gray-800 mb-6">Iniciar Sesión</h1>

        <div className="bg-white shadow w-full rounded-lg divide-y divide-gray-200">
          {error ? (
            <span className="p-4 block bg-red-500 text-white text-center rounded-t-lg">
              {message}
            </span>
          ) : (
            ""
          )}
        </div>

        <form onSubmit={formik.handleSubmit}>
          <div className="mb-4">
            <label htmlFor="email" className="block text-gray-700 font-semibold mb-2">
              Correo electrónico
            </label>
            <input 
              type="email"
              name="email"
              id="email"
              value={formik.values.email}
              onChange={formik.handleChange}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Ingresa tu correo electrónico"
            />
            {formik.touched.email && formik.errors.email && (
              <div className="text-red-500 text-xs mb-2">
                {formik.errors.email}
              </div>
            )}
          </div>

          <div className="mb-4">
            <label htmlFor="password" className="block text-gray-700 font-semibold mb-2">
              Contraseña
            </label>
            <input 
              type="password"
              name="password"
              id="password"
              value={formik.values.password}
              onChange={formik.handleChange}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Ingresa tu contraseña"
            />
            {formik.touched.password && formik.errors.password && (
              <div className="text-red-500 text-xs mb-2">
                {formik.errors.password}
              </div>
            )}
          </div>

          <button 
            type="submit"
            className="w-full py-2 px-4 bg-gray-500 text-white font-semibold rounded-lg hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
          >
            Iniciar Sesión
          </button>
        </form>
      </div>
    </div>
  )
}


