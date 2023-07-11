using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;

namespace ChiChien.UI
{
    public class LoginUI : UIBase
    {
        public InputField emailInput;
        public InputField passwordInput;
        public InputField passwordConfirmInput;
        public InputField nickName;
        public Button loginButton;
        public Button signInButton;
        public Button ResendButton;
        public Text InfoText;

        public Auth.AuthManager authCtrler;

        private AuthResult _authResult;
        private Auth.AuthManager.Response _signInResponse;

        private const int timeout = 6000000;

        public override void Initalize() {
            if (this.gameObject.activeSelf) this.gameObject.SetActive(false);
            this.authCtrler = new Auth.AuthManager();
        }
        
        public override void ShowUI() {
            if (!this.gameObject.activeSelf) this.gameObject.SetActive(true);
            loginButton.onClick.AddListener(OnLoginButtonClick);
            signInButton.onClick.AddListener(OnSigninBtnClick);
            ResendButton.onClick.AddListener(OnResendBtnClick);
        }

        public override void CloseUI() {
            if (this.gameObject.activeSelf) this.gameObject.SetActive(false);
            loginButton.onClick.RemoveListener(OnLoginButtonClick);
            signInButton.onClick.RemoveListener(OnSigninBtnClick);
            ResendButton.onClick.RemoveListener(OnResendBtnClick);
        }

        private async void OnSigninBtnClick() {
            string email = emailInput.text;
            string password = passwordInput.text;
            string confirm = passwordConfirmInput.text;

            if (!ComparePassword(password, confirm)) {
                PrintInfo("The confirmation password is not identical");
                return;
            }

            if (string.IsNullOrEmpty(nickName.text)) {
                PrintInfo("Please Enter Nick Name");
                return;
            }

            var response = authCtrler.SignIn(email, password);
            if (await Task.WhenAny(response, Task.Delay(timeout)) == response) {
                this._signInResponse = response.Result;
                LoginCallback(response.Result);               
            } else {
                PrintInfo("Resquest Timeout");
            }
            Debug.Log("TEST");

        }

        private async void OnLoginButtonClick() {
            string email = emailInput.text;
            string password = passwordInput.text;
            
            var response = authCtrler.Login(email, password);
            if(await Task.WhenAny(response, Task.Delay(timeout)) == response) {
                LoginCallback(response.Result);
            } else {
                PrintInfo("Resquest Timeout");
            }
        }

        private void OnResendBtnClick() {
            if (this._authResult != null) {
                authCtrler.SendVerificationEmail(this._authResult);
                SendVerificationEmailProcess(this._signInResponse);
            }
        }

        private async void CheckVerificationLogin() {
            string email = emailInput.text;
            string password = passwordInput.text;
            var response = authCtrler.Login(email, password);
            if (await Task.WhenAny(response, Task.Delay(4000)) == response) {
                if (response != null) {
                    if (response.Result.isSuccess) {
                        if (response.Result.authResult.User.IsEmailVerified) {
                            Debug.Log("Email is Verified");
                            this._authResult = response.Result.authResult;
                            PrintInfo("Loading...");
                        }
                    }
                }
            }
        }

        private void LoginCallback(Auth.AuthManager.Response response) {
            if (response != null) {
                if (response.isSuccess) {
                    Debug.Log("Login Get UID " + response.authResult.User.UserId);
                    this._authResult = response.authResult;
                    if (!response.authResult.User.IsEmailVerified) {
                        SendVerificationEmailProcess(response);
                    } else {
                        PrintInfo("Loading...");
                        Core.DataCenter.Instance.InitalDBData(this._authResult.User.UserId, GoToNextProc);
                    }
                    
                } else {
                    if (response.exception != null)
                        LoginErrorHandler(response.exception);
                }

            }
        }

        private async void SendVerificationEmailProcess(Auth.AuthManager.Response response) {
            authCtrler.SendVerificationEmail(response.authResult);
            SwitchToEmailVerfication();
            string friendUid = Guid.NewGuid().ToString();
            await Task.Run(() => Core.DataCenter.Instance.CreateNewUserData(
                    this._signInResponse.authResult.User.Email,
                    this._signInResponse.authResult.User.UserId,
                    this.nickName.text,
                    friendUid
                ));
            await Task.Run(() => { Core.DataCenter.Instance.CreateFriendData(friendUid, null); });
            StartCoroutine(CheckEmailVerification(GoToNextProc));
        }

        private void GoToNextProc() {
            Debug.Log("GoToNextScene");
            Core.GameManager.Instance.ChangeProc(Core.ProcStatus.Friend);
        }

        private void SwitchToLogin() {
            emailInput.gameObject.SetActive(true);
            passwordInput.gameObject.SetActive(true);
            passwordConfirmInput.gameObject.SetActive(false);
            nickName.gameObject.SetActive(false);
            signInButton.gameObject.SetActive(false);
            loginButton.gameObject.SetActive(true);
            ResendButton.gameObject.SetActive(false);
        } 

        private void SwitchToSignIn() {
            emailInput.gameObject.SetActive(true);
            passwordInput.gameObject.SetActive(true);
            passwordConfirmInput.gameObject.SetActive(true);
            nickName.gameObject.SetActive(true);
            signInButton.gameObject.SetActive(true);
            loginButton.gameObject.SetActive(false);
            ResendButton.gameObject.SetActive(false);
        }

        private void SwitchToEmailVerfication() {
            emailInput.gameObject.SetActive(false);
            passwordInput.gameObject.SetActive(false);
            passwordConfirmInput.gameObject.SetActive(false);
            nickName.gameObject.SetActive(false);
            signInButton.gameObject.SetActive(false);
            loginButton.gameObject.SetActive(false);
            ResendButton.gameObject.SetActive(true);
            ResendButton.interactable = false;
            PrintInfo("Please Verify Your Email Verification email has been sent You can Resend after 5min");
        }

        private void LoginErrorHandler(FirebaseException e) {
            
            AuthError authError = (AuthError)e.ErrorCode;
            switch (authError) {
                case AuthError.InvalidEmail:
                    PrintInfo("Email is invalid");
                    break;
                case AuthError.WrongPassword:
                    PrintInfo("Wrong Password");
                    break;
                case AuthError.MissingEmail:
                    PrintInfo("Email is missing");
                    break;
                case AuthError.MissingPassword:
                    PrintInfo("Password is missing");
                    break;
                case AuthError.UserNotFound:
                    PrintInfo("Your Are New User");
                    SwitchToSignIn();
                    break;
                default:
                    PrintInfo("Login Failed");
                    break;
            }
        }

        private void PrintInfo(string info) {
            InfoText.text = info;
        }

        private bool ComparePassword(string password , string confirmPassword) {
            return string.Compare(password, confirmPassword) == 0;
        }

        private IEnumerator CheckEmailVerification(Action callback) {
            float timer = 0;
            bool doOnceGate = false;

            while (timer < 300) {
                timer += Time.deltaTime;
                if (((int)timer) % 5 == 0) {
                    if (!doOnceGate) {
                        if (this._authResult.User.IsEmailVerified) {
                            callback.Invoke();
                            yield break;
                        } else {
                            Debug.Log("Verification Uncheck");
                        }
                        CheckVerificationLogin();
                        doOnceGate = true;
                    }
                } else {
                    doOnceGate = false;
                }
                yield return null;
            }
            ResendButton.interactable = true;
        }
    }
}