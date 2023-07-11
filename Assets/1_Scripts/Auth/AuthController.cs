using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System;
using System.Threading.Tasks;

namespace ChiChien.Auth{
    public class AuthManager
    {
        public class Response
        {
            public bool isSuccess;
            public AuthResult authResult;
            public FirebaseException exception;
            public Response(bool isSuccess, AuthResult authResult, FirebaseException exception = null) {
                this.isSuccess = isSuccess;
                this.authResult = authResult;
                this.exception = exception;
            }
        }

        public class EmailResponse
        {
            public bool isSuccess;
            public FirebaseException exception;
            public EmailResponse(bool isSuccess, FirebaseException exception = null) {
                this.isSuccess = isSuccess;
                this.exception = exception;
            }
        }

        private FirebaseAuth auth;
        private FirebaseUser user;

        public AuthManager() {
            InitalFirebaseAuth();
        }

        private void InitalFirebaseAuth() {
            this.auth = FirebaseAuth.DefaultInstance;
        }


        public async Task<Response> Login(string email, string password) {
            Task<AuthResult> t = auth.SignInWithEmailAndPasswordAsync(email, password);
            try {
                AuthResult result = await t;
                return new Response(true, result, null);
            } catch (AggregateException e){
                FirebaseException firebaseException = e.GetBaseException() as FirebaseException;
                //AuthError authError = (AuthError)firebaseException.ErrorCode;
                return new Response(false, null, firebaseException);
            }

            
        }

        public async Task<Response> SignIn(string email, string password) {
            Task<AuthResult> t = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            try {
                AuthResult result = await t;
                return new Response(true, result, null);

            } catch (AggregateException e) {
                FirebaseException firebaseException = e.GetBaseException() as FirebaseException;
                //AuthError authError = (AuthError)firebaseException.ErrorCode;
                return new Response(false, null, firebaseException);
            }


        }

        public async void SendVerificationEmail(AuthResult result) {
            if (result.User != null) {
                try {
                    await result.User.SendEmailVerificationAsync();
                }catch(AggregateException e) {
                    FirebaseException firebaseException = e.GetBaseException() as FirebaseException;
                    Debug.Log(firebaseException);
                }
                
            }
            
        }

        private void Signout() {
            auth.SignOut();
            Debug.Log("User signed out.");
        }
    }


}
