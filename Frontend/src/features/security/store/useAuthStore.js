import { create } from "zustand";
import { loginAsync } from "../../../shared/actions/auth";
import { jwtDecode } from "jwt-decode";

export const useAuthStore = create((set, get) => ({
  user: null,
  token: null,
  roles: [],
  isAuthenticated: false,
  message: "",
  error: false,

  // Iniciar Sesión
  login: async (form) => {
    const { status, data, message } = await loginAsync(form);

    if (status) {
      set({
        error: false,
        user: {
            email: data.email,
            tokenExpiration: data.tokenExpiration,
        },
        token: data.token,
        isAuthenticated: true,
        message: message
      });

      // Guardar información en LocalStorage
      localStorage.setItem('user', JSON.stringify(get().user ?? {}));
      localStorage.setItem('token', get().token);

      return;
    }

    // Si status es falso... 
    set({message: message, error: true});
    return;
  },

  // Registrar usuario
  // register: async (form) => {
  //   try {
  //     const { status, message } = await registerAsync(form);

  //     if (status) {
  //       await get().login({ email: form.email, password: form.password });
  //       set({ message: "Usuario registrado correctamente." });
  //     } else {
  //       set({ error: true, message: message });
  //     }
  //   } catch (error) {
  //     console.error("Error en el registro:", error.message);
  //     set({ error: true, message: "Ocurrió un error durante el registro." });
  //   }
  // },

  // Renovar el token
  setSession: (user, token, refreshToken) => {
    set({user: user, token: token, refreshToken: refreshToken, isAuthenticated: true});

    localStorage.setItem('user', JSON.stringify(get().user ?? {}));
    localStorage.setItem('token', get().token);
    localStorage.setItem('refreshToken', get().refreshToken);
  },

  // Cerrar Sesión
  logout: () => {
    set({
      user: null,
      token: null,
      isAuthenticated: false,
      error: false,
      message: "",
      roles: []
    });
    localStorage.clear();
  },

  // Validar la sesión
  validateAuthentication: () => {
    const token = localStorage.getItem('token') ?? '';

    if(token === ''){
      set({isAuthenticated: false});
      return ;
    }else{
      try {
        const decodeJwt = jwtDecode(token);
        const currentTime = Math.floor(Date.now()/1000);
        // TODO: Cambiar con el refreshTokenExpire
        if (decodeJwt.exp < currentTime) {
          console.log('Token expirado');
          set({isAuthenticated: false});
          return;
        }

        const roles = decodeJwt["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ?? [];

        set({isAuthenticated: true, roles: typeof(roles) === 'string' ? [roles] : roles});
      } catch (error) {
        console.error(error);
        set({isAuthenticated: false});
      }
    }
  },

  // Obtener el id del usuario autenticado decodificando el token
  getUserId: () => {
    const token = localStorage.getItem("token");

    if (!token) {
      console.warn("No hay token disponible. El usuario no está autenticado.");
      return null;
    }

    try {
      const decodedToken = jwtDecode(token);
      return decodedToken.UserId ?? null;
    } catch (error) {
      console.error("Error al decodificar el token:", error.message);
      return null;
    }
  },
}));
